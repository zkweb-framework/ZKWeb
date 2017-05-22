using System;
using System.Text;
using System.Reflection;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Exception extension methods<br/>
	/// 例外的扩展函数<br/>
	/// </summary>
	public static class ExceptionExtensions {
		/// <summary>
		/// Get detailed information from exception<br/>
		/// 获取例外的详细信息<br/>
		/// </summary>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var ex = new Exception("qwert", new Exception("inner qwert"));
		/// var str = ex.ToDetailedString();
		/// </code>
		/// </example>
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
		/// Get summary information from exception<br/>
		/// 获取例外的简要信息<br/>
		/// </summary>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var ex = new Exception("qwert", new Exception("inner qwert"));
		/// var str = ex.ToSummaryString();
		/// </code>
		/// </example>
		public static string ToSummaryString(this Exception ex) {
			while (ex.InnerException != null) {
				ex = ex.InnerException;
			}
			return ex.Message;
		}
	}
}
