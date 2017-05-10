using System;
using System.Text;
using System.Reflection;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Exception extension methods
	/// </summary>
	public static class ExceptionExtensions {
		/// <summary>
		/// Get detailed information from exception
		/// </summary>
		/// <returns></returns>
		public static string ToDetailedString(this Exception ex) {
			var messageBuilder = new StringBuilder();
			messageBuilder.AppendLine(ex.ToString());
			while (ex != null) {
				switch (ex) {
					case ReflectionTypeLoadException rex:
						messageBuilder.AppendLine("LoaderExceptions:");
						foreach (var loaderException in rex.LoaderExceptions) {
							messageBuilder.Append(loaderException.ToDetailedString());
						}
						break;
				}
				ex = ex.InnerException;
			}
			return messageBuilder.ToString();
		}

		/// <summary>
		/// Get summary information from exception
		/// </summary>
		/// <returns></returns>
		public static string ToSummaryString(this Exception ex) {
			while (ex.InnerException != null) {
				ex = ex.InnerException;
			}
			return ex.Message;
		}
	}
}
