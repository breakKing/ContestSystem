using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace ContestSystem.Logger
{
    public class FileLogger: ILogger
    {
        private readonly string _filePathPrefix;
        private static object _lock = new object();

        public FileLogger(string path)
        {
            _filePathPrefix = path;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                lock (_lock)
                {
                    File.AppendAllText(GenerateFileName(DateTime.UtcNow), formatter(state, exception) + Environment.NewLine);
                }
            }
        }

        public string GetLogEntries(long lastDaysCount)
        {
            string entries = "";
            if (lastDaysCount <= 0)
            {
                return entries;
            }
            DateTime now = DateTime.UtcNow;
            DateTime startDate = now.AddDays(-(lastDaysCount - 1));
            while (startDate <= now)
            {
                string fileName = GenerateFileName(startDate);
                if (File.Exists(fileName))
                {
                    entries += '\n' + File.ReadAllText(fileName);
                }
                startDate.AddDays(1);
            }
            entries = entries.Trim();
            return entries;
        }

        private string GenerateFileName(DateTime dateTime)
        {
            string dateString = dateTime.ToString("dd-MM-yyyy");
            return $"{_filePathPrefix}_{dateString}.txt";
        }
    }
}
