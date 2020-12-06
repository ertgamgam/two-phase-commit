using Microsoft.Extensions.Logging;

namespace TwoPhaseCommitCoordinator
{
    public static class ApplicationLogging
    {
        public static ILoggerFactory LogFactory()
        {
            return LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
            });
        }

        public static ILogger<T> CreateLogger<T>() => LogFactory().CreateLogger<T>();
    }
}