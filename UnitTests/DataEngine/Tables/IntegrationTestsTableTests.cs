using BuildHub.Common.Logger;
using BuildHub.DataEngine.DatabaseConnection;
using BuildHub.DataEngine.DatabaseConnectionManager;
using BuildHub.DataEngine.Exceptions.Entities;
using BuildHub.DataEngine.Queries;
using BuildHub.DataEngine.Transactions;
using UnitTests.DataEngine.Common;

namespace UnitTests.DataEngineTests.Tables
{
	[TestClass]
	[TestCategory("Integration")]
	public class IntegrationTestsTableTests
	{
		public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            Logger.Initialize();
            DatabaseConnectionManager.GetInstance().Initialize();
        }

        [ClassCleanup]
		public static void Cleanup()
		{
		}

		[TestMethod]
		public void Construct_Table_Should_Not_Be_Null()
		{
			var integrationTestsTable = new IntegrationTestsTable();
			Assert.IsNotNull(integrationTestsTable);
		}

		[TestMethod]
		public void Get_All_Unit_Test_Should_Not_Be_Null()
		{
			var integrationTestsTable = new IntegrationTestsTable();
			var uniTests = integrationTestsTable.GetAll();

			Assert.IsNotNull(uniTests);
		}

		[TestMethod]
		public void Assert_Get_By_Guid_Returns_Null_If_Entity_Does_Not_Exist()
		{
			var integrationTestTable = new IntegrationTestsTable();
			Assert.IsNull(integrationTestTable.GetByGuid(Guid.NewGuid()));
		}

		[TestMethod]
		public void Get_Unit_Test_By_Guid()
		{
			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = this.TestContext.TestName;

			var integrationTestTable = new IntegrationTestsTable();
			Assert.IsTrue(integrationTestTable.Insert(integrationTest));

			Assert.IsGreaterThan(0, integrationTestTable.GetByGuid(integrationTest.Guid).Id);
		}

		[TestMethod]
		public void Get_All_Unit_Test_With_Unmapped_Fields_Should_Throw_Exception()
		{
            var integrationTest = new IntegrationTestEntity();
            integrationTest.Name = this.TestContext.TestName;

            var integrationTestTable = new IntegrationTestsTable();
			Assert.IsTrue(integrationTestTable.Insert(integrationTest));

            var integrationTestTableWithUnmappedFields = new InegrationTestWithUnmappedFieldTable();
			Assert.Throws<MissingColumnDescriptionException>(() => integrationTestTableWithUnmappedFields.GetAll());
		}

		[TestMethod]
		public void Get_By_Unit_Test_By_Condition()
		{
			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = this.TestContext.TestName;
			var integrationTestTable = new IntegrationTestsTable();

			Assert.IsTrue(integrationTestTable.Insert(integrationTest));
			Assert.IsNotNull(integrationTestTable.GetByCondition(integrationTest, (integrationTest) => integrationTest.Id));
		}

		[TestMethod]
		public void Get_By_Unit_Test_By_Condition_With_Query_Builder()
		{
			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = this.TestContext.TestName;
			var integrationTestTable = new IntegrationTestsTable();

			Assert.IsTrue(integrationTestTable.Insert(integrationTest));

			QueryBuilder queryBuilder = new QueryBuilder()
				.Where(integrationTest, (integrationTest) => integrationTest.Guid);

			Assert.IsNotNull(integrationTestTable.GetByCondition(queryBuilder));
		}

		[TestMethod]
		public void Assert_Insert_Unit_Test()
		{
			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = this.TestContext.TestName;

			var integrationTestTable = new IntegrationTestsTable();
			Assert.IsTrue(integrationTestTable.Insert(integrationTest));

			var dbUnitTest = integrationTestTable.GetByGuid(integrationTest.Guid);
			Assert.AreEqual(integrationTest.Guid, dbUnitTest.Guid);
		}

		[TestMethod]
		public void Assert_That_Inserting_Duplicate_Unit_Test_Should_Throw()
		{
			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = this.TestContext.TestName;

			var integrationTestTable = new IntegrationTestsTable();
			Assert.IsTrue(integrationTestTable.Insert(integrationTest));
			Assert.IsFalse(integrationTestTable.Insert(integrationTest));
		}

		[TestMethod]
		public void Assert_Update_Unit_Test_Is_True()
		{
			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = this.TestContext.TestName;

			var integrationTestTable = new IntegrationTestsTable();
			Assert.IsTrue(integrationTestTable.Insert(integrationTest));

			integrationTest.Name = this.TestContext.TestName.ToLower();
			Assert.IsTrue(integrationTestTable.Update(integrationTest));
		}

		[TestMethod]
		public void Assert_Delete_Unit_Test_Is_True()
		{
			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = this.TestContext.TestName;

			var integrationTestTable = new IntegrationTestsTable();
			Assert.IsTrue(integrationTestTable.Insert(integrationTest));

			integrationTest.Name = this.TestContext.TestName;
			Assert.IsTrue(integrationTestTable.Delete(integrationTest));
		}

		[TestMethod]
		public void Deleting_An_Existing_Unit_Test_Should_Return_False()
		{
			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = this.TestContext.TestName;

			var integrationTestTable = new IntegrationTestsTable();
			Assert.IsTrue(integrationTestTable.Insert(integrationTest));

			integrationTest.Name = this.TestContext.TestName;
			Assert.IsTrue(integrationTestTable.Delete(integrationTest));

			Assert.IsFalse(integrationTestTable.Delete(integrationTest));
		}

		[TestMethod]
		public void Assert_That_Unit_Test_Entity_Without_Table_Name_attribute_Throws()
		{
			Assert.Throws<MissingTableNameException>(() => new IntegrationTestWithoutTableNameAttributeTable());
		}

        [TestMethod]
        public void Get_All_Should_Return_Empty_List_When_No_Records_Exist()
        {
            var integrationTestsTable = new IntegrationTestsTable();
            var allTests = integrationTestsTable.GetAll();
            Assert.IsNotNull(allTests);
            Assert.AreEqual(0, allTests.Count());
        }

        [TestMethod]
        public void Insert_Should_Persist_Name_Correctly()
        {
            var integrationTest = new IntegrationTestEntity();
            integrationTest.Name = this.TestContext.TestName;

            var integrationTestsTable = new IntegrationTestsTable();
            Assert.IsTrue(integrationTestsTable.Insert(integrationTest));

            var dbEntity = integrationTestsTable.GetByGuid(integrationTest.Guid);
            Assert.AreEqual(this.TestContext.TestName, dbEntity.Name);
        }

        [TestMethod]
        public void Insert_Should_Assign_Valid_Id_After_Insert()
        {
            var integrationTest = new IntegrationTestEntity();
            integrationTest.Name = this.TestContext.TestName;

            var integrationTestsTable = new IntegrationTestsTable();
            Assert.IsTrue(integrationTestsTable.Insert(integrationTest));
            Assert.IsGreaterThan(0, integrationTest.Id);
        }

        [TestMethod]
        public void Insert_Should_Assign_Unique_Guids_For_Different_Entities()
        {
            var integrationTest1 = new IntegrationTestEntity();
            integrationTest1.Name = this.TestContext.TestName + "_1";

            var integrationTest2 = new IntegrationTestEntity();
            integrationTest2.Name = this.TestContext.TestName + "_2";

            var integrationTestsTable = new IntegrationTestsTable();
            Assert.IsTrue(integrationTestsTable.Insert(integrationTest1));
            Assert.IsTrue(integrationTestsTable.Insert(integrationTest2));

            Assert.AreNotEqual(integrationTest1.Guid, integrationTest2.Guid);
        }

        [TestMethod]
        public void Update_Should_Persist_Changed_Name()
        {
            var integrationTest = new IntegrationTestEntity();
            integrationTest.Name = this.TestContext.TestName;

            var integrationTestsTable = new IntegrationTestsTable();
            Assert.IsTrue(integrationTestsTable.Insert(integrationTest));

            string updatedName = this.TestContext.TestName + "_updated";
            integrationTest.Name = updatedName;
            Assert.IsTrue(integrationTestsTable.Update(integrationTest));

            var dbEntity = integrationTestsTable.GetByGuid(integrationTest.Guid);
            Assert.AreEqual(updatedName, dbEntity.Name);
        }

        [TestMethod]
        public void Update_Should_Return_False_For_Non_Existing_Entity()
        {
            var integrationTest = new IntegrationTestEntity();
            integrationTest.Name = this.TestContext.TestName;

            var integrationTestsTable = new IntegrationTestsTable();
            Assert.IsFalse(integrationTestsTable.Update(integrationTest));
        }

        [TestMethod]
        public void Delete_Should_Remove_Entity_From_Database()
        {
            var integrationTest = new IntegrationTestEntity();
            integrationTest.Name = this.TestContext.TestName;

            var integrationTestsTable = new IntegrationTestsTable();
            Assert.IsTrue(integrationTestsTable.Insert(integrationTest));
            Assert.IsTrue(integrationTestsTable.Delete(integrationTest));

            Assert.IsNull(integrationTestsTable.GetByGuid(integrationTest.Guid));
        }

        [TestMethod]
        public void Delete_Non_Existing_Entity_Should_Return_False()
        {
            var integrationTest = new IntegrationTestEntity();
            integrationTest.Name = this.TestContext.TestName;

            var integrationTestsTable = new IntegrationTestsTable();
            Assert.IsFalse(integrationTestsTable.Delete(integrationTest));
        }

        [TestMethod]
        public void Get_All_Should_Return_All_Inserted_Entities()
        {
            var integrationTestsTable = new IntegrationTestsTable();
            int initialCount = integrationTestsTable.GetAll().Count();

            var test1 = new IntegrationTestEntity { Name = this.TestContext.TestName + "_1" };
            var test2 = new IntegrationTestEntity { Name = this.TestContext.TestName + "_2" };
            var test3 = new IntegrationTestEntity { Name = this.TestContext.TestName + "_3" };

            integrationTestsTable.Insert(test1);
            integrationTestsTable.Insert(test2);
            integrationTestsTable.Insert(test3);

            Assert.AreEqual(initialCount + 3, integrationTestsTable.GetAll().Count());
        }

        [TestMethod]
        public void Get_By_Condition_With_Query_Builder_Should_Return_Empty_List_For_Non_Existing_Guid()
        {
            var integrationTestsTable = new IntegrationTestsTable();
            var nonExistingEntity = new IntegrationTestEntity { Name = this.TestContext.TestName };

            QueryBuilder queryBuilder = new QueryBuilder()
                .Where(nonExistingEntity, (e) => e.Guid);

            Assert.IsEmpty(integrationTestsTable.GetByCondition(queryBuilder));
        }

        [TestMethod]
        public void Get_By_Condition_Should_Return_Correct_Entity()
        {
            var integrationTest = new IntegrationTestEntity();
            integrationTest.Name = this.TestContext.TestName;

            var integrationTestsTable = new IntegrationTestsTable();
            Assert.IsTrue(integrationTestsTable.Insert(integrationTest));

            var result = integrationTestsTable.GetByGuid(integrationTest.Guid);
            Assert.IsNotNull(result);
            Assert.AreEqual(integrationTest.Guid, result.Guid);
        }

        [TestMethod]
        public void Insert_Multiple_Entities_Should_All_Be_Retrievable()
        {
            var integrationTestsTable = new IntegrationTestsTable();
            var entities = new List<IntegrationTestEntity>();

            for (int i = 0; i < 5; i++)
            {
                var entity = new IntegrationTestEntity { Name = $"{this.TestContext.TestName}_{i}" };
                Assert.IsTrue(integrationTestsTable.Insert(entity));
                entities.Add(entity);
            }

            foreach (var entity in entities)
            {
                var dbEntity = integrationTestsTable.GetByGuid(entity.Guid);
                Assert.IsNotNull(dbEntity);
                Assert.AreEqual(entity.Guid, dbEntity.Guid);
            }
        }

        [TestMethod]
        public void Insert_Within_Transaction_Should_Rollback_On_Failure()
        {
            var integrationTestsTable = new IntegrationTestsTable();
            IntegrationTestEntity integrationTest = null;

            try
            {
                using var scopedTransaction = new ScopedTransaction(DatabaseSource.IntegrationTests);

                integrationTest = new IntegrationTestEntity { Name = this.TestContext.TestName };
                Assert.IsTrue(integrationTestsTable.Insert(integrationTest));

            }
            catch { }

            Assert.IsNull(integrationTestsTable.GetByGuid(integrationTest.Guid));
        }

        [TestMethod]
        public void Insert_Within_Committed_Transaction_Should_Persist()
        {
            var integrationTestsTable = new IntegrationTestsTable();
            var integrationTest = new IntegrationTestEntity { Name = this.TestContext.TestName };

            using (var scopedTransaction = new ScopedTransaction(DatabaseSource.IntegrationTests))
            {
                Assert.IsTrue(integrationTestsTable.Insert(integrationTest));
                scopedTransaction.Commit();
            }

            Assert.IsNotNull(integrationTestsTable.GetByGuid(integrationTest.Guid));
        }
    }
}
