using System;
using System.Threading;
using ZKWebStandard.Collections;
using ZKWebStandard.Web.Mock;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Http管理器
	/// </summary>
	public static class HttpManager {
		/// <summary>
		/// 获取当前的上下文
		/// 不存在时抛出例外
		/// </summary>
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
		/// 判断当前的上下文是否存在
		/// </summary>
		public static bool CurrentContextExists { get { return currentContext.Value != null; } }

		/// <summary>
		/// 重载当前的Http上下文
		/// 结束后恢复原有的上下文
		/// </summary>
		/// <param name="context">Http上下文</param>
		/// <returns></returns>
		public static IDisposable OverrideContext(IHttpContext context) {
			var original = currentContext.Value;
			currentContext.Value = context;
			return new SimpleDisposable(() => currentContext.Value = original);
		}

		/// <summary>
		/// 重载当前的Http上下文
		/// 结束后恢复原有的上下文
		/// </summary>
		/// <param name="pathAndQuery">路径和请求字符串，不以"/"开头时自动补上</param>
		/// <param name="method">请求类型，GET或POST</param>
		/// <returns></returns>
		public static IDisposable OverrideContext(string pathAndQuery, string method) {
			return OverrideContext(new HttpContextMock(pathAndQuery, method));
		}
	}
}
