
namespace BuildHub.Infrastructure.Services.Startup.Logger
{
    using BuildHub.Common.Logger;

    /// <summary>
    /// 
    /// </summary>
    public sealed class LoggerStartupService : ILoggerStartupService
    {
        public bool InitializeLogger()
        {
            try
            {
                Logger.Initialize();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
