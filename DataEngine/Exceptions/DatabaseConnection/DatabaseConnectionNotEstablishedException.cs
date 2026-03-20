using BuildHub.DataEngine.DatabaseConnection;

namespace BuildHub.DataEngine.Exceptions.DatabaseConnection
{
    /// <summary>
    /// Exception class thrown when we are trying to access a connenction which is not established 
    /// </summary>
    public sealed class DatabaseConnectionNotEstablishedException : Exception
    {
        /// <summary>
        /// Database source
        /// </summary>
        public DatabaseSource DatabaseSource { get; set; }

        public DatabaseConnectionNotEstablishedException(DatabaseSource databaseSource) : base()
        {
            this.DatabaseSource = databaseSource;
        }
    }
}
