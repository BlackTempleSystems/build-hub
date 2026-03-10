namespace BuildHub.Domain.Services.Database
{
    /// <summary>
    /// Provides methods for performing maintenance tasks on a database, such as backups and optimizations.
    /// </summary>
    /// <remarks>This service is designed to facilitate routine database maintenance operations, ensuring
    /// optimal performance and reliability. It may include features for scheduling tasks and monitoring maintenance
    /// activities.</remarks>
    public interface IDatabaseMaintananceService
    {
        /// <summary>
        /// Checks if the required database are reachable
        /// </summary>
        /// <returns>returns whether the required databases are reachable.</returns>
        public bool IsDatabaseRachable();
    }
}
