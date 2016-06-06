using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ZKWeb.Utils.Collections;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Utils.Functions {
	/// <summary>
	/// http上下文的工具类
	/// 功能
	/// - 提供返回HttpContextBase的CurrentContext
	/// - 提供获取和储存数据到当前http上下文的Items的函数，如果上下文不存在则储存到线程本地的集合中
	/// - 提供获取和储存数据到当前http上下文的Cookies的函数，如果上下文不存在则储存到线程本地的集合中
	/// - 获取客户端的Ip地址
	/// - 获取请求时使用的域名地址
	/// - 重载当前使用的http上下文
	/// </summary>
	public static class HttpContextUtils {
		/// <summary>
		/// 当前的http上下文
		/// </summary>
		public static HttpContextBase CurrentContext {
			get {
				if (_overrideContext.Value != null) {
					return _overrideContext.Value;
				} else if (HttpContext.Current != null) {
					return new HttpContextWrapper(HttpContext.Current);
				}
				return null;
			}
		}
		/// <summary>
		/// 重载当前的http上下文
		/// </summary>
		private static ThreadLocal<HttpContextBase> _overrideContext = new ThreadLocal<HttpContextBase>();
		/// <summary>
		/// 当前的http上下文等于null时的备用数据储存
		/// </summary>
		private static ThreadLocal<Dictionary<string, object>> ItemsFallback { get; set; }
		= new ThreadLocal<Dictionary<string, object>>(() => new Dictionary<string, object>());
		/// <summary>
		/// 当前的http上下文等于null时的备用Cookies储存
		/// </summary>
		private static ThreadLocal<Dictionary<string, string>> CookiesFallback { get; set; }
		= new ThreadLocal<Dictionary<string, string>>(() => new Dictionary<string, string>());

		/// <summary>
		/// 储存在一个Http请求中通用的数据
		/// 如果上下文不存在则储存到线程本地的集合中
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="key">键值</param>
		/// <param name="data">数据</param>
		public static void PutData<T>(string key, T data)
			where T : class {
			var context = CurrentContext;
			if (context == null) {
				ItemsFallback.Value[key] = data;
			} else {
				context.Items[key] = data;
			}
		}

		/// <summary>
		/// 获取在一个Http请求中通用的数据
		/// 如果上下文不存在则从线程本地的集合中获取
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="key">键值</param>
		/// <param name="defaultValue">获取不到时返回的默认值</param>
		/// <returns></returns>
		public static T GetData<T>(string key, T defaultValue = default(T))
			where T : class {
			var context = CurrentContext;
			if (context == null) {
				return (ItemsFallback.Value.GetOrDefault(key) as T) ?? defaultValue;
			} else {
				return (context.Items[key] as T) ?? defaultValue;
			}
		}

		/// <summary>
		/// 获取在一个Http请求中通用的数据，不存在时创建
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="key">键值</param>
		/// <param name="defaultCreater">获取不到时创建默认值使用的函数</param>
		/// <returns></returns>
		public static T GetOrCreateData<T>(string key, Func<T> defaultCreater)
			where T : class {
			var value = GetData<T>(key);
			if (value == null) {
				value = defaultCreater();
				PutData(key, value);
			}
			return value;
		}

		/// <summary>
		/// 删除在一个Http请求中通用的数据
		/// 如果上下文不存在则从线程本地的集合中删除
		/// </summary>
		/// <param name="key">键值</param>
		public static void RemoveData(string key) {
			var context = CurrentContext;
			if (context == null) {
				ItemsFallback.Value.Remove(key);
			} else {
				context.Items.Remove(key);
			}
		}

		/// <summary>
		/// 获取Cookie值
		/// 如果上下文不存在则从线程本地的集合中获取
		/// </summary>
		/// <param name="key">键值</param>
		/// <returns></returns>
		public static string GetCookie(string key) {
			// 当前上下文不存在时使用备用Cookies储存
			var context = CurrentContext;
			if (context == null) {
				return CookiesFallback.Value.GetOrDefault(key);
			}
			// 如果在Get之前调用了Put，会使用Response中的值
			// 否则使用Request中的值
			HttpCookie cookie = null;
			if (context.Response.Cookies.AllKeys.Contains(key)) {
				cookie = context.Response.Cookies[key]; // 直接访问会创建空值
			} else {
				cookie = context.Request.Cookies[key];
			}
			if (string.IsNullOrEmpty(cookie?.Value)) {
				return null;
			}
			return HttpUtility.UrlDecode(cookie.Value);
		}

		/// <summary>
		/// 设置Cookie值
		/// 如果上下文不存在则储存到线程本地的集合中
		/// </summary>
		/// <param name="key">键值</param>
		/// <param name="value">数据</param>
		/// <param name="expired">过期时间，null表示在浏览器关闭时过期</param>
		/// <param name="httpOnly">是否只能通过http获取（javascript中不能获取）</param>
		/// <returns></returns>
		public static bool PutCookie(string key, string value,
			DateTime? expired = null, bool httpOnly = false) {
			// 当前上下文不存在时使用备用Cookies储存
			var context = CurrentContext;
			if (context == null) {
				if (expired.HasValue && expired.Value.Year <= 1970) {
					CookiesFallback.Value.Remove(key);
				} else {
					CookiesFallback.Value[key] = value;
				}
				return true;
			}
			// 如果在Get之前调用了Put，会使用Response中的值
			// 否则使用Request中的值
			HttpCookie cookie = null;
			if (context.Response.Cookies.AllKeys.Contains(key)) {
				cookie = context.Response.Cookies[key];
			} else {
				cookie = context.Request.Cookies[key] ?? new HttpCookie(key);
			}
			// 设置Cookie值
			// 过期时间等于DateTime.MinValue时会在浏览器关闭后删除
			cookie.Expires = expired.HasValue ? expired.Value : DateTime.MinValue;
			cookie.HttpOnly = httpOnly;
			cookie.Value = HttpUtility.UrlEncode(value);
			try {
				context.Response.Cookies.Remove(key);
				context.Response.Cookies.Add(cookie);
				return true;
			} catch {
				return false; // 连接中断时这里会抛例外
			}
		}

		/// <summary>
		/// 删除Cookie值
		/// 如果上下文不存在则从线程本地的集合中删除
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool RemoveCookie(string key) {
			return PutCookie(key, "", new DateTime(1970, 1, 1));
		}

		/// <summary>
		/// 获取客户端的Ip地址
		/// </summary>
		/// <returns></returns>
		public static string GetClientIpAddress() {
			return CurrentContext?.Request?.UserHostAddress ?? "::1";
		}

		/// <summary>
		/// 获取请求时使用的域名地址
		/// 例: "http://localhost" 后面不带/
		/// </summary>
		/// <returns></returns>
		public static string GetRequestHostUrl() {
			var context = CurrentContext;
			if (context == null) {
				return "http://localhost";
			}
			return context.Request.Url.GetLeftPart(UriPartial.Authority);
		}

		/// <summary>
		/// 重载当前使用的http上下文
		/// 结束后恢复原有的上下文
		/// </summary>
		/// <param name="context">指定的http上下文</param>
		/// <returns></returns>
		public static IDisposable OverrideContext(HttpContextBase context) {
			var original = _overrideContext.Value;
			_overrideContext.Value = context;
			return new SimpleDisposable(() => _overrideContext.Value = original);
		}

		/// <summary>
		/// 重载当前使用的http上下文
		/// 结束后恢复原有的上下文
		/// </summary>
		/// <param name="uri">请求的uri</param>
		/// <param name="method">请求类型，GET或POST等</param>
		/// <returns></returns>
		public static IDisposable OverrideContext(Uri uri, string method) {
			var mockContext = new HttpContextMock();
			var mockRequest = new HttpRequestMock();
			var mockResponse = new HttpResponseMock();
			mockContext.request = mockRequest;
			mockContext.response = mockResponse;
			// 设置请求参数
			mockRequest.url = uri;
			mockRequest.path = uri.AbsolutePath;
			mockRequest.httpMethod = method;
			if (method == "GET") {
				mockRequest.queryString = HttpUtility.ParseQueryString(uri.Query);
			} else if (method == "POST") {
				mockRequest.form = HttpUtility.ParseQueryString(uri.Query);
			}
			// 当前请求存在时，继承Items, Cookies, Headers
			var exists = CurrentContext;
			if (exists != null) {
				mockContext.items = exists.Items;
				mockRequest.cookies = exists.Request.Cookies;
				mockRequest.headers = exists.Request.Headers;
			}
			return OverrideContext(mockContext);
		}

		/// <summary>
		/// 重载当前使用的http上下文
		/// 结束后恢复原有的上下文
		/// </summary>
		/// <param name="path">请求的路径，不需要带域名</param>
		/// <param name="method">请求类型，GET或POST</param>
		/// <returns></returns>
		public static IDisposable OverrideContext(string path, string method) {
			var url = "http://localhost" + (path.StartsWith("/") ? "" : "/") + path;
			return OverrideContext(new Uri(url), method);
		}
	}
}
