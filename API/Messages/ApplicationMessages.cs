namespace BuildHub.API.Messages
{
    internal static class ApplicationMessages
    {
        public readonly static string APPLICATION_STARTING = "========================================";
        public readonly static string APPLICATION_HEADER = "  BuildHub API";
        public readonly static string APPLICATION_ENVIRONMENT = "  Environment  : {0}";
        public readonly static string APPLICATION_VERSION = "  Version       : {0}";
        public readonly static string APPLICATION_SEPARATOR = "========================================";
        public readonly static string APPLICATION_STARTED = "  Status        : Running";
        public readonly static string APPLICATION_LISTENING = "  Listening on  : {0}";
        public readonly static string APPLICATION_SHUTDOWN = "  Status        : Shutting down gracefully";
    }
}
