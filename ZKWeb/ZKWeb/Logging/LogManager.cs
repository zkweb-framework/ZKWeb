using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using ZKWeb.Storage;

namespace ZKWeb.Logging
{
    /// <summary>
    /// Log manager<br/>
    /// 日志管理器<br/>
    /// </summary>
    /// <example>
    /// <code language="cs">
    /// var logManager = Application.Ioc.Resolve&lt;LogManager&gt;();
    /// logManager.LogDebug("debug message");
    /// </code>
    /// </example>
    public class LogManager
    {
        /// <summary>
        /// The format of date time part in filename<br/>
        /// 文件名中的时间格式<br/>
        /// </summary>
        public static readonly string DateFormatInFilename = "yyyyMMdd";
        /// <summary>
        /// The format of date time part in log message<br/>
        /// 日志中的时间格式<br/>
        /// </summary>
        public static readonly string DateFormatInLog = "yyyy/MM/dd HH:mm:ss";

        /// <summary>
        /// Log message to file<br/>
        /// 记录日志到文件<br/>
        /// </summary>
        public virtual void Log(string filename, string message)
        {
            // Write to console
            Console.Write(message);
            // Write to log file
            // Retry up to 100 times if inconsistency occurs between the threads
            var fileStorage = Application.Ioc.Resolve<IFileStorage>();
            var logFile = fileStorage.GetStorageFile("logs", filename);
            for (int n = 0; n < 100; ++n)
            {
                try
                {
                    logFile.AppendAllText(message);
                    break;
                }
                catch (IOException)
                {
                    Thread.Sleep(5);
                }
            }
        }

        /// <summary>
        /// Log debug level message<br/>
        /// 记录除错等级的信息<br/>
        /// </summary>
        public virtual void LogDebug(string message,
            [CallerMemberName] string memberName = null)
        {
            var now = DateTime.UtcNow.ToLocalTime();
            var filename = $"Debug.{now.ToString(DateFormatInFilename)}.log";
            Log(filename, $"{now.ToString(DateFormatInLog)} ({memberName}) {message}\r\n");
        }

        /// <summary>
        /// Log information level message<br/>
        /// 记录一般等级的信息<br/>
        /// </summary>
        public virtual void LogInfo(string message,
            [CallerMemberName] string memberName = null)
        {
            var now = DateTime.UtcNow.ToLocalTime();
            var filename = $"Info.{now.ToString(DateFormatInFilename)}.log";
            Log(filename, $"{now.ToString(DateFormatInLog)} ({memberName}) {message}\r\n");
        }

        /// <summary>
        /// Log error level message<br/>
        /// 记录错误等级的信息<br/>
        /// </summary>
        public virtual void LogError(string message,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            var now = DateTime.UtcNow.ToLocalTime();
            var filename = $"Error.{now.ToString(DateFormatInFilename)}.log";
            Log(filename, $"{now.ToString(DateFormatInLog)} ({filePath}:{lineNumber} {memberName}) {message}\r\n");
        }

        /// <summary>
        /// Log transaction (releated to money) message<br/>
        /// 记录交易(关系到钱财的)信息<br/>
        /// </summary>
        public virtual void LogTransaction(string message,
            [CallerMemberName] string memberName = null)
        {
            var now = DateTime.UtcNow.ToLocalTime();
            var filename = $"Transaction.{now.ToString(DateFormatInFilename)}.log";
            Log(filename, $"{now.ToString(DateFormatInLog)} ({memberName}) {message}\r\n");
        }
    }
}
