namespace UnitTests.DataEngineTests.SQLQueries
{
	using BuildHub.DataEngine.Queries;
	using UnitTests.DataEngineTests.Tables;

	[TestClass]
	public sealed class QueryBuilderStateTests
	{
		[TestMethod]
		public void New_Query_Builder_State_Should_Use_Defaults()
		{
			var state = new QueryBuilderState();

			Assert.AreEqual(0, state.WhereStatements.Count);
			Assert.AreEqual(LockTypes.None, state.LockType);
			Assert.AreEqual(-1, state.TopStatementCount);
		}

		[TestMethod]
		public void Reset_Should_Clear_Conditions_And_Restore_Defaults()
		{
			var state = new QueryBuilderState();
			state.WhereStatements.Add(new WhereCondition("NAME", CompareTypes.Equal, "BuildHub"));
			state.LockType = LockTypes.Update;
			state.TopStatementCount = 10;

			state.Reset();

			Assert.AreEqual(0, state.WhereStatements.Count);
			Assert.AreEqual(LockTypes.None, state.LockType);
			Assert.AreEqual(-1, state.TopStatementCount);
		}

		[TestMethod]
		public void Query_Builder_Should_Update_Top_Lock_And_Where_State()
		{
			var queryBuilder = new QueryBuilder()
				.Top(5)
				.Lock(LockTypes.Update)
				.Where("name", CompareTypes.NotEqual, "BuildHub");

			var condition = queryBuilder.QueryBuilderState.WhereStatements.Single();

			Assert.AreEqual(5, queryBuilder.QueryBuilderState.TopStatementCount);
			Assert.AreEqual(LockTypes.Update, queryBuilder.QueryBuilderState.LockType);
			Assert.AreEqual("NAME", condition.InternalWhereCondition.Item1);
			Assert.AreEqual(CompareTypes.NotEqual, condition.InternalWhereCondition.Item2);
			Assert.AreEqual("BuildHub", condition.InternalWhereCondition.Item3);
			Assert.AreEqual(WhereConditionTypes.WhereConditionTypeAnd, condition.WhereConditionType);
		}

		[TestMethod]
		public void Query_Builder_Expression_Where_Should_Use_Entity_Column_Metadata()
		{
			var queryBuilder = new QueryBuilder()
				.Where<IntegrationTestEntity>(entity => entity.Name, "BuildHub");

			var condition = queryBuilder.QueryBuilderState.WhereStatements.Single();

			Assert.AreEqual("NAME", condition.InternalWhereCondition.Item1);
			Assert.AreEqual(CompareTypes.Equal, condition.InternalWhereCondition.Item2);
			Assert.AreEqual("BuildHub", condition.InternalWhereCondition.Item3);
			Assert.AreEqual(WhereConditionTypes.WhereConditionTypeAnd, condition.WhereConditionType);
		}

		[TestMethod]
		public void Query_Builder_Where_Or_With_Entity_Should_Store_Compiled_Value()
		{
			var entity = new IntegrationTestEntity()
			{
				Name = "BuildHub"
			};

			var queryBuilder = new QueryBuilder()
				.OrWhere(entity, currentEntity => currentEntity.Name, CompareTypes.NotEqual);

			var condition = queryBuilder.QueryBuilderState.WhereStatements.Single();

			Assert.AreEqual("NAME", condition.InternalWhereCondition.Item1);
			Assert.AreEqual(CompareTypes.NotEqual, condition.InternalWhereCondition.Item2);
			Assert.AreEqual("BuildHub", condition.InternalWhereCondition.Item3);
			Assert.AreEqual(WhereConditionTypes.WhereConditionTypeOr, condition.WhereConditionType);
		}

		[TestMethod]
		public void Where_Condition_Should_Expose_Internal_Condition_Data()
		{
			var condition = new WhereCondition("NAME", CompareTypes.Equal, "BuildHub", WhereConditionTypes.WhereConditionTypeOr);

			Assert.AreEqual("NAME", condition.InternalWhereCondition.Item1);
			Assert.AreEqual(CompareTypes.Equal, condition.InternalWhereCondition.Item2);
			Assert.AreEqual("BuildHub", condition.InternalWhereCondition.Item3);
			Assert.AreEqual(WhereConditionTypes.WhereConditionTypeOr, condition.WhereConditionType);
		}
	}
}
