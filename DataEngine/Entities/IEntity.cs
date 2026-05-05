namespace BuildHub.DataEngine.Entities
{
	/// <summary>
	/// Interface of en database entity 
	/// </summary>
	public interface IEntity
	{
		/// <summary>
		/// Unique numeric identifier. (Internal)
		/// </summary>
		public int Id { get; }

		/// <summary>
		/// Unique numeric identifier. (External)
		/// </summary>
		public Guid Guid { get; }
	}
}
