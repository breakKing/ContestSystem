using ContestSystem.Logger;
using Microsoft.Extensions.Logging;

namespace ContestSystem.Extensions
{
    public static class LoggerExtension
    {
        public static ILoggerFactory AddFile(this ILoggerFactory factory, string filePath)
        {
            factory.AddProvider(new FileLoggerProvider(filePath));
            return factory;
        }
    }
}
