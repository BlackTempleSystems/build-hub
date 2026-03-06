namespace BuildHub.Infrastructure.Services.Database.Startup
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDatabaseStartupService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool SetupDatabaseConnections();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CloseDatabaseConnections();
    }
}
