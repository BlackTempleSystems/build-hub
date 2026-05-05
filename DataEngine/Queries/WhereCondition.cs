namespace BuildHub.DataEngine.Queries
{
	/// <summary>
	/// Specifies the logical operator to use when combining multiple conditions in a WHERE clause.
	/// </summary>
	/// <remarks>Use this enumeration to indicate whether conditions should be combined using a logical AND or OR.
	/// This is typically used when constructing dynamic queries or filtering data based on multiple criteria.</remarks>
	public enum WhereConditionTypes
	{
		WhereConditionTypeAnd,
		WhereConditionTypeOr
	}

	/// <summary>
	/// Represents a single condition used in a SQL WHERE clause, including the column, comparison type, and value to
	/// compare against.
	/// </summary>
	/// <remarks>Use this class to construct query conditions programmatically when building dynamic SQL statements
	/// or query expressions. The condition type determines how this condition is combined with others (for example, using
	/// AND or OR).</remarks>
	public class WhereCondition
	{
		/// <summary>
		/// Gets the internal representation of the WHERE condition used for filtering data.
		/// </summary>
		public Tuple<string, CompareTypes, object?> InternalWhereCondition { get; private set; }

		/// <summary>
		/// Gets the type of the WHERE condition to apply when building queries.
		/// </summary>

		public WhereConditionTypes WhereConditionType { get; private set; }

		public WhereCondition(string columnName, CompareTypes compareType, object? value, WhereConditionTypes conditionTypes = WhereConditionTypes.WhereConditionTypeAnd)
		{
			this.InternalWhereCondition = new Tuple<string, CompareTypes, object?>(columnName, compareType, value);
		}
	
	}
}
