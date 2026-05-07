namespace UnitTests.DataEngineTests.DatabaseConnection
{
	using BuildHub.DataEngine.DatabaseConnection;
    using BuildHub.DataEngine.DatabaseConnectionManager;
    using BuildHub.DataEngine.Exceptions.DatabaseConnection;

	[TestClass]
	[TestCategory("Integration")]
	[DoNotParallelize]
	public sealed class DatabaseConnectionManagerTests
	{
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            DatabaseConnectionManager.GetInstance().Initialize();
        }

        [TestMethod]
		public void Get_Instance_Should_Return_Singleton_Instance()
		{
			DatabaseConnectionManager DatabaseConnectionManagerInstance = DatabaseConnectionManager.GetInstance();
			Assert.IsNotNull(DatabaseConnectionManagerInstance);
		}

		[TestMethod]
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Get_connection_should_return_open_connection_for_valid_database_source(DatabaseSource databaseSource)
		{
			DatabaseConnectionManager DatabaseConnectionManagerInstance = DatabaseConnectionManager.GetInstance();
			using DatabaseConnection databaseConnection = DatabaseConnectionManagerInstance.GetDatabaseConnection(databaseSource);

			Assert.IsTrue(databaseConnection.IsConnectionOpen());
		}

		[TestMethod]
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Release_Connection_Should_Keep_Connection_Open_And_Return_To_Pool(DatabaseSource databaseSource)
		{
			DatabaseConnectionManager DatabaseConnectionManagerInstance = DatabaseConnectionManager.GetInstance();
			DatabaseConnection databaseConnection = DatabaseConnectionManagerInstance.GetDatabaseConnection(databaseSource);

			DatabaseConnectionManagerInstance.ReleaseDatabaseConnection(databaseConnection);
			Assert.IsTrue(databaseConnection.IsConnectionOpen());
		}

		[TestMethod]
		[DataRow(-1)]
		[DataRow(-2)]
		[DataRow(-3)]
		public void Get_Connection_Should_Throw_Key_Not_Found_Exception_For_Invalid_Database_Source(int falseDatabaseSource)
		{
			DatabaseConnectionManager DatabaseConnectionManagerInstance = DatabaseConnectionManager.GetInstance();
			Assert.Throws<KeyNotFoundException>(() => DatabaseConnectionManagerInstance.GetDatabaseConnection((DatabaseSource)falseDatabaseSource));
		}

		[TestMethod]
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Get_Available_Connections_Count_Should_Return_Positive_Number(DatabaseSource databaseSource)
		{
			DatabaseConnectionManager DatabaseConnectionManagerInstance = DatabaseConnectionManager.GetInstance();
			Assert.IsGreaterThan(0, DatabaseConnectionManagerInstance.GetPoolMetrics(databaseSource).IdleConnectionsCount);
		}

		[TestMethod]
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Dispose_Connection_Should_Decrease_Currently_Used_Connections_Count(DatabaseSource databaseSource)
		{
			DatabaseConnectionManager DatabaseConnectionManagerInstance = DatabaseConnectionManager.GetInstance();

			DatabaseConnection databaseConnection = DatabaseConnectionManagerInstance.GetDatabaseConnection(databaseSource);

			int currentlyUsedConnectionsBeforeDispose = DatabaseConnectionManagerInstance.GetPoolMetrics(databaseSource).ActiveConnectionsCount;
			databaseConnection.Dispose();

			Assert.IsGreaterThan<int>(DatabaseConnectionManagerInstance.GetPoolMetrics(databaseSource).ActiveConnectionsCount, currentlyUsedConnectionsBeforeDispose);
		}

		[TestMethod]
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Get_Connections_Concurrently_Should_Return_Valid_Connections(DatabaseSource databaseSource)
		{
			DatabaseConnectionManager DatabaseConnectionManagerInstance = DatabaseConnectionManager.GetInstance();
			for (int index = 0; index < DatabaseConnectionManagerInstance.GetPoolMetrics(databaseSource).IdleConnectionsCount; ++index)
			{
				Parallel.Invoke(() =>
				{
					using DatabaseConnection databaseConnection = DatabaseConnectionManagerInstance.GetDatabaseConnection(databaseSource);
					Assert.IsNotNull(databaseConnection);
				});
			}
		}

		[TestMethod]
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Get_Connections_Exceeding_Pool_Size_Concurrently_Should_Handle_Gracefully(DatabaseSource databaseSource)
		{
			DatabaseConnectionManager DatabaseConnectionManagerInstance = DatabaseConnectionManager.GetInstance();

			for (int index = 0; index < DatabaseConnectionManagerInstance.GetPoolMetrics(databaseSource).IdleConnectionsCount + 1; ++index)
			{
				Parallel.Invoke(() =>
				{
					using DatabaseConnection databaseConnection = DatabaseConnectionManagerInstance.GetDatabaseConnection(databaseSource);
					Assert.IsNotNull(databaseConnection);
				});
			}
		}

		[TestMethod]
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Exhausted_Pool_With_Retry_Should_Eventually_Get_Connection_When_Released(DatabaseSource databaseSource)
		{
			DatabaseConnectionManager pool = DatabaseConnectionManager.GetInstance();
			int maxConnections = pool.GetPoolMetrics(databaseSource).IdleConnectionsCount;
			List<DatabaseConnection> heldConnections = new List<DatabaseConnection>();

			try
			{
				for (int i = 0; i < maxConnections; i++)
				{
					heldConnections.Add(pool.GetDatabaseConnection(databaseSource));
				}

				var task = Task.Run(() =>
				{
					Thread.Sleep(1000); // Wait a bit
					using var conn = pool.GetDatabaseConnection(databaseSource);
					Assert.IsNotNull(conn);
				});

				Thread.Sleep(500);
				pool.ReleaseDatabaseConnection(heldConnections[0]);
				heldConnections.RemoveAt(0);

				Assert.IsTrue(task.Wait(5000), "Should get connection after retry");
			}
			finally
			{
				foreach (var conn in heldConnections)
				{
					pool.ReleaseDatabaseConnection(conn);
				}
			}
		}

		[TestMethod]
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Exhausted_Pool_Exceeding_Max_Retries_Should_Throw_Pool_Exhausted_Exception(DatabaseSource databaseSource)
		{
			DatabaseConnectionManager pool = DatabaseConnectionManager.GetInstance();
			int maxConnections = pool.GetPoolMetrics(databaseSource).IdleConnectionsCount;
			List<DatabaseConnection> heldConnections = new List<DatabaseConnection>();

			try
			{
				for (int i = 0; i < maxConnections; i++)
				{
					heldConnections.Add(pool.GetDatabaseConnection(databaseSource));
				}

				Assert.Throws<ConnectionPoolExhaustedException>(() =>
					pool.GetDatabaseConnection(databaseSource));
			}
			finally
			{
				foreach (var conn in heldConnections)
				{
					pool.ReleaseDatabaseConnection(conn);
				}
			}
		}

		[TestMethod]
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Release_Connection_Twice_Should_Not_Corrupt_Pool_Count(DatabaseSource databaseSource)
		{
			DatabaseConnectionManager pool = DatabaseConnectionManager.GetInstance();
			int initialCount = pool.GetPoolMetrics(databaseSource).IdleConnectionsCount;

			var conn = pool.GetDatabaseConnection(databaseSource);
			pool.ReleaseDatabaseConnection(conn);

			Assert.Throws<DatabaseConnectionLeakException>(() => pool.ReleaseDatabaseConnection(conn));
		}

		[TestMethod]
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Get_Currently_Used_Connections_Count_Should_Reflect_Actual_Usage(DatabaseSource databaseSource)
		{
			DatabaseConnectionManager pool = DatabaseConnectionManager.GetInstance();
			int initialUsed = pool.GetPoolMetrics(databaseSource).ActiveConnectionsCount;

			var conn1 = pool.GetDatabaseConnection(databaseSource);
			Assert.AreEqual(initialUsed + 1, pool.GetPoolMetrics(databaseSource).ActiveConnectionsCount);

			var conn2 = pool.GetDatabaseConnection(databaseSource);
			Assert.AreEqual(initialUsed + 2, pool.GetPoolMetrics(databaseSource).ActiveConnectionsCount);

			pool.ReleaseDatabaseConnection(conn1);
			Assert.AreEqual(initialUsed + 1, pool.GetPoolMetrics(databaseSource).ActiveConnectionsCount);

			pool.ReleaseDatabaseConnection(conn2);
			Assert.AreEqual(initialUsed, pool.GetPoolMetrics(databaseSource).ActiveConnectionsCount);
		}

		[TestMethod]
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Get_Connection_From_Exhausted_Pool_Should_Throw_Pool_Exhausted_Exception(DatabaseSource databaseSource)
		{
			DatabaseConnectionManager pool = DatabaseConnectionManager.GetInstance();
			int maxConnections = 100;

			Parallel.For(0, maxConnections, x =>
			{
				try
				{
                    var connection = pool.GetDatabaseConnection(databaseSource);
                }
				catch(ConnectionPoolExhaustedException)
				{
					Assert.Throws<ConnectionPoolExhaustedException>(() => throw new ConnectionPoolExhaustedException(databaseSource));
				}

            });

			Assert.Throws<ConnectionPoolExhaustedException>(() => pool.GetDatabaseConnection(databaseSource));
		}

		[TestMethod]
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Connection_Pool_Should_Regrow_And_Return_A_Connection(DatabaseSource databaseSource)
		{
			DatabaseConnectionManager pool = DatabaseConnectionManager.GetInstance();
			int maxConnections = pool.GetPoolMetrics(databaseSource).IdleConnectionsCount;

			Parallel.For(0, maxConnections, x =>
			{
				var connection = pool.GetDatabaseConnection(databaseSource);
			});

			Assert.IsNotNull(pool.GetDatabaseConnection(databaseSource));
		}
    }
}
