using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using ZKWeb.Model;

namespace ZKWeb.Core {
	/// <summary>
	/// 获取日志管理器
	/// </summary>
	public class LogManager {
		/// <summary>
		/// 记录日志
		/// </summary>
		public static void Log(string filename, string message) {
			// 创建日志文件夹
			var logsDirectory = PathConfig.LogsDirectory;
			Directory.CreateDirectory(logsDirectory);
			// 写入到控制台
			Console.Write(message);
			// 写入到日志文件
			// 有可能因为多个进程同时打开失败，最多重试100次
			var now = DateTime.UtcNow.ToLocalTime();
			var path = Path.Combine(logsDirectory, filename);
			for (int n = 0; n < 100; ++n) {
				try {
					File.AppendAllText(path, message, Encoding.UTF8);
					break;
				} catch (IOException) {
					Thread.Sleep(5);
				}
			}
		}

		/// <summary>
		/// 记录除错级别的日志
		/// </summary>
		public void LogDebug(string message,
			[CallerMemberName] string memberName = null) {
			var now = DateTime.UtcNow.ToLocalTime();
			var filename = $"Debug.{now.ToString("yyyyMMdd")}.log";
			Log(filename, $"{now.ToString()} ({memberName}) {message}\r\n");
		}

		/// <summary>
		/// 记录消息级别的日志
		/// </summary>
		public void LogInfo(string message,
			[CallerMemberName] string memberName = null) {
			var now = DateTime.UtcNow.ToLocalTime();
			var filename = $"Info.{now.ToString("yyyyMMdd")}.log";
			Log(filename, $"{now.ToString()} ({memberName}) {message}\r\n");
		}

		/// <summary>
		/// 记录错误级别的日志
		/// </summary>
		public void LogError(string message,
			[CallerMemberName] string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0) {
			var now = DateTime.UtcNow.ToLocalTime();
			var filename = $"Error.{now.ToString("yyyyMMdd")}.log";
			Log(filename, $"{now.ToString()} ({filePath}:{lineNumber} {memberName}) {message}\r\n");
		}

		/// <summary>
		/// 记录交易日志
		/// </summary>
		public void LogTransaction(string message,
			[CallerMemberName] string memberName = null) {
			var now = DateTime.UtcNow.ToLocalTime();
			var filename = $"Transaction.{now.ToString("yyyyMMdd")}.log";
			Log(filename, $"{now.ToString()} ({memberName}) {message}\r\n");
		}
	}
}
