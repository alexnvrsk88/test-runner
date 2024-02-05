using System;
using System.Threading.Tasks;
using Grace.Extend;
using UnityEngine;

namespace Runner.Core.Services.Logging
{   
    /// <summary>
    /// Сервис для логирования
    /// </summary>
    [Injection(true, typeof(ILoggingService))]
    public sealed class LoggingService : ServiceAbstract, ILoggingService
    {
        public override Task<bool> Initialize()
        {
            return Task.FromResult(true);
        }
        
        public void Info(string message)
        {
            Debug.Log(message);
        }
        
        public void Warning(string message)
        {
            Debug.LogWarning(message);
        }
        
        public void Error(string message)
        {
            Debug.LogError(message);
        }
        
        public void Exception(Exception exception, string message)
        {
            if (string.IsNullOrEmpty(message) == false)
            {
                Debug.LogError(message);
            }
            
            Debug.LogException(exception);
        }
    }
}
