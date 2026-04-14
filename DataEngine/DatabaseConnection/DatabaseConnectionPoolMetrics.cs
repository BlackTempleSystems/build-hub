namespace BuildHub.DataEngine.DatabaseConnection
{
    /// <summary>
    /// Represents a snapshot of metrics for a database connection pool, including connection counts and pending
    /// requests.
    /// </summary>
    /// <remarks>Use this class to monitor the current state of a database connection pool, such as the number
    /// of active, idle, and total connections, as well as the number of requests waiting for a connection. This
    /// information can be useful for diagnostics and performance tuning.</remarks>
    public sealed class DatabaseConnectionPoolMetrics
    {
        /// <summary>
        /// Total opened connections.
        /// </summary>
        private int _totalConnectionsCount;

        /// <summary>
        /// Active connections currently in use.
        /// </summary>
        private int _activeConnectionsCount;

        /// <summary>
        /// Open connections which are not used at the moment.
        /// </summary>
        private int _idleConnectionsCount;

        /// <summary>
        /// Total request waiting to retrieve a connection.
        /// </summary>
        private int _waitingRequestsCount;

        /// <summary>
        /// Gets the total number of connections currently tracked by the instance.
        /// </summary>
        public int TotalConnectionsCount => Volatile.Read(ref _totalConnectionsCount);

        /// <summary>
        /// 
        /// </summary>
        public int ActiveConnectionsCount => Volatile.Read(ref _activeConnectionsCount);

        /// <summary>
        /// 
        /// </summary>
        public int IdleConnectionsCount => Volatile.Read(ref _idleConnectionsCount);

        /// <summary>
        /// 
        /// </summary>
        public int WaitingRequestsCount => Volatile.Read(ref _waitingRequestsCount);

        /// <summary>
        /// Notifies the pool that a new connection has been created and updates internal connection counters.
        /// </summary>
        /// <remarks>This method is typically called when a new connection is established and added to the
        /// pool. It is thread-safe and can be called concurrently from multiple threads.</remarks>
        internal void OnCreate()
        {
            Interlocked.Increment(ref _totalConnectionsCount);
            Interlocked.Increment(ref _idleConnectionsCount);
        }

        /// <summary>
        /// Updates the internal counters to reflect that a connection has been acquired and is now active.
        /// </summary>
        /// <remarks>This method is intended to be called when a connection transitions from idle to
        /// active state. It is thread-safe and should be used in scenarios where connection pooling or resource
        /// tracking is required.</remarks>
        internal void OnAcquire()
        {
            Interlocked.Decrement(ref _idleConnectionsCount);
            Interlocked.Increment(ref _activeConnectionsCount);
        }

        /// <summary>
        /// Releases a connection and updates the active and idle connection counts accordingly.
        /// </summary>
        /// <remarks>Call this method when a connection is no longer in use to ensure accurate tracking of
        /// connection pool state. This method is thread-safe.</remarks>
        internal void OnRelease()
        {
            Interlocked.Decrement(ref _totalConnectionsCount);
            Interlocked.Decrement(ref _activeConnectionsCount);
            Interlocked.Increment(ref _idleConnectionsCount);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void OnClose()
        {
            Interlocked.Decrement(ref _totalConnectionsCount);
            Interlocked.Decrement(ref _idleConnectionsCount);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void OnWaitStart() => Interlocked.Increment(ref _waitingRequestsCount);

        /// <summary>
        /// 
        /// </summary>
        internal void OnWaitEnd() => Interlocked.Decrement(ref _waitingRequestsCount);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double CalculateUtilization()
        {
            return (_activeConnectionsCount / _totalConnectionsCount) * 100;
        }
    }
}
