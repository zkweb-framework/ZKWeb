using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ZKWeb.ORM.EFCore {
	/// <summary>
	/// Logger provider for EFCore<br/>
	/// EFCore日志记录器的提供器<br/>
	/// see: https://docs.microsoft.com/en-us/ef/core/miscellaneous/logging <br/>
	/// </summary>
	public class EFCoreLoggerProvider : ILoggerProvider {
		/// <summary>
		/// The context<br/>
		/// 数据库上下文<br/>
		/// </summary>
		protected EFCoreDatabaseContext Context { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public EFCoreLoggerProvider(EFCoreDatabaseContext context) {
			Context = context;
		}

		/// <summary>
		/// Get logger<br/>
		/// 获取日志记录器<br/>
		/// </summary>
		public ILogger CreateLogger(string categoryName) {
			if (categoryName == typeof(IRelationalCommandBuilderFactory).FullName) {
				return new Logger(Context);
			}
			return NullLogger.Instance;
		}

		/// <summary>
		/// Do nothing<br/>
		/// 不做任何事情<br/>
		/// </summary>
		public void Dispose() {
		}

		/// <summary>
		/// The logger pass text to IDatabaseCommandLogger if present<br/>
		/// 用于传递文本给IDatabaseCommandLogger(如果存在)的日志记录器<br/>
		/// </summary>
		protected class Logger : ILogger {
#pragma warning disable CS1591
			protected EFCoreDatabaseContext Context { get; set; }

			public Logger(EFCoreDatabaseContext context) {
				Context = context;
			}

			public IDisposable BeginScope<TState>(TState state) {
				return null;
			}

			public bool IsEnabled(LogLevel logLevel) {
				return true;
			}

			public void Log<TState>(LogLevel logLevel, EventId eventId,
				TState state, Exception exception, Func<TState, Exception, string> formatter) {
				var commandLogger = Context.CommandLogger;
				if (commandLogger != null) {
					commandLogger.LogCommand(Context, formatter(state, exception), new { state, exception });
				}
			}
#pragma warning restore CS1591
		}
	}
}
