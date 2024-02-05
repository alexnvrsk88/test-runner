using System;

namespace Runner.Core.Services.Logging
{
    public interface ILoggingService
    {
        void Info(string message);
        void Error(string message);
        void Warning(string message);
        void Exception(Exception exception, string message = null);
    }
}
