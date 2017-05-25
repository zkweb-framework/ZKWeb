using System;
using System.Threading;
using ZKWebStandard.Collections;
using ZKWebStandard.Web.Mock;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Http manager<br/>
	/// Http管理器<br/>
	/// </summary>
	public static class HttpManager {
		/// <summary>
		/// Get using http context<br/>
		/// Throw exception if not exist<br/>
		/// 获取当前的Http上下文<br/>
		/// 如果不存在则抛出例外<br/>
		/// </summary>
		/// <example>
		/// <code language="cs">
		/// var context = HttpManager.CurrentContext;
		/// </code>
		/// </example>
		public static IHttpContext CurrentContext {
			get {
				var context = currentContext.Value;
				if (context == null) {
					throw new NullReferenceException("Context does not exists");
				}
				return context;
			}
		}
		private static ThreadLocal<IHttpContext> currentContext = new ThreadLocal<IHttpContext>();
		/// <summary>
		/// Determines if there a http context is using<br/>
		/// 检测当前是否有Http上下文<br/>
		/// </summary>
		public static bool CurrentContextExists { get { return currentContext.Value != null; } }

		/// <summary>
		/// Override using http context<br/>
		/// Restore to previous context after disposed<br/>
		/// 重载当前的Http上下文<br/>
		/// Dispose后恢复为原来的上下文<br/>
		/// </summary>
		/// <param name="context">Http context</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var context = new MyContext();
		/// using (HttpManager.OverrideContext(context)) {
		/// }
		/// </code>
		/// </example>
		public static IDisposable OverrideContext(IHttpContext context) {
			var original = currentContext.Value;
			currentContext.Value = context;
			return new SimpleDisposable(() => {
				// check again to avoid gc dispose
				if (currentContext.Value == context) {
					currentContext.Value = original;
				}
			});
		}

		/// <summary>
		/// Override using http context<br/>
		/// Restore to previous context after disposed<br/>
		/// 重载当前的Http上下文<br/>
		/// Dispose后恢复为原来的上下文<br/>
		/// </summary>
		/// <param name="pathAndQuery">Path and query, "/" will be automatic added to front if needed</param>
		/// <param name="method">Method, GET or POST</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// using (HttpManager.OverrideContext("abc", "GET")) {
		/// }
		/// </code>
		/// </example>
		public static IDisposable OverrideContext(string pathAndQuery, string method) {
			return OverrideContext(new HttpContextMock(pathAndQuery, method));
		}
	}
}
