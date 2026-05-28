namespace UnitTests.DataEngineTests.DatabaseConnection
{
	using BuildHub.DataEngine.DatabaseConnection;

	[TestClass]
	public sealed class DatabaseConnectionPoolMetricsTests
	{
		[TestMethod]
		public void New_Metrics_Should_Start_With_Zero_Counts()
		{
			var metrics = new DatabaseConnectionPoolMetrics();

			Assert.AreEqual(0, metrics.TotalConnectionsCount);
			Assert.AreEqual(0, metrics.ActiveConnectionsCount);
			Assert.AreEqual(0, metrics.IdleConnectionsCount);
			Assert.AreEqual(0, metrics.WaitingRequestsCount);
		}

		[TestMethod]
		public void Create_Acquire_And_Release_Should_Update_Connection_Counts()
		{
			var metrics = new DatabaseConnectionPoolMetrics();

			metrics.OnCreate();
			metrics.OnCreate();
			metrics.OnAcquire();
			metrics.OnRelease();

			Assert.AreEqual(2, metrics.TotalConnectionsCount);
			Assert.AreEqual(0, metrics.ActiveConnectionsCount);
			Assert.AreEqual(2, metrics.IdleConnectionsCount);
		}

		[TestMethod]
		public void Close_Should_Remove_An_Idle_Connection_From_Total_Count()
		{
			var metrics = new DatabaseConnectionPoolMetrics();

			metrics.OnCreate();
			metrics.OnClose();

			Assert.AreEqual(0, metrics.TotalConnectionsCount);
			Assert.AreEqual(0, metrics.IdleConnectionsCount);
		}

		[TestMethod]
		public void Wait_Start_And_End_Should_Update_Waiting_Request_Count()
		{
			var metrics = new DatabaseConnectionPoolMetrics();

			metrics.OnWaitStart();
			Assert.AreEqual(1, metrics.WaitingRequestsCount);

			metrics.OnWaitEnd();
			Assert.AreEqual(0, metrics.WaitingRequestsCount);
		}

		[TestMethod]
		public void Calculate_Utilization_Should_Return_Zero_When_No_Connections_Exist()
		{
			var metrics = new DatabaseConnectionPoolMetrics();

			Assert.AreEqual(0.0, metrics.CalculateUtilizationPercentage());
		}

		[TestMethod]
		public void Calculate_Utilization_Should_Use_Double_Division()
		{
			var metrics = new DatabaseConnectionPoolMetrics();

			metrics.OnCreate();
			metrics.OnCreate();
			metrics.OnAcquire();

			Assert.AreEqual(50.0, metrics.CalculateUtilizationPercentage(), 0.001);
		}
	}
}
