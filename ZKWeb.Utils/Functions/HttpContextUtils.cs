using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Utils.Functions {
	/// <summary>
	/// HttpContext的工具类
	/// </summary>
	public static class HttpContextUtils {
		/// <summary>
		/// HttpContext.Current等于null时的备用数据储存
		/// </summary>
		internal static ThreadLocal<Dictionary<string, object>> ItemsFallback { get; set; }
		= new ThreadLocal<Dictionary<string, object>>(() => new Dictionary<string, object>());

		/// <summary>
		/// HttpContext.Current等于null时的备用Cookies储存
		/// </summary>
		internal static ThreadLocal<Dictionary<string, string>> CookiesFallback { get; set; }
		= new ThreadLocal<Dictionary<string, string>>(() => new Dictionary<string, string>());

		/// <summary>
		/// 储存在一个Http请求中通用的数据
		/// 用于代替ViewData，因为ViewData每次描画分视图时都会复制一遍耗费性能
		/// </summary>
		public static void PutData<T>(string plugin, string name, T data)
			where T : class {
			var context = HttpContext.Current;
			var key = $"{plugin}.{name}";
			if (context == null) {
				ItemsFallback.Value[key] = data;
			} else {
				context.Items[key] = data;
			}
		}

		/// <summary>
		/// 获取在一个Http请求中通用的数据
		/// </summary>
		public static T GetData<T>(
			string plugin, string name, T defaultValue = default(T))
			where T : class {
			var context = HttpContext.Current;
			var key = $"{plugin}.{name}";
			if (context == null) {
				return (ItemsFallback.Value.GetOrDefault(key) as T) ?? defaultValue;
			} else {
				return (context.Items[$"{plugin}.{name}"] as T) ?? defaultValue;
			}
		}

		/// <summary>
		/// 获取在一个Http请求中通用的数据，不存在时创建
		/// </summary>
		public static T GetOrCreateData<T>(
			string plugin, string name, Func<T> defaultCreater)
			where T : class {
			var value = GetData<T>(plugin, name);
			if (value == null) {
				value = defaultCreater();
				PutData(plugin, name, value);
			}
			return value;
		}

		/// <summary>
		/// 删除在一个Http请求中通用的数据
		/// </summary>
		public static void RemoveData(string plugin, string name) {
			var context = HttpContext.Current;
			var key = $"{plugin}.{name}";
			if (context == null) {
				ItemsFallback.Value.Remove(key);
			} else {
				context.Items.Remove(key);
			}
		}

		/// <summary>
		/// 获取客户端的Ip地址
		/// </summary>
		/// <returns></returns>
		public static string GetClientIpAddress() {
			return HttpContext.Current?.Request?.UserHostAddress ?? "::1";
		}

		/// <summary>
		/// 获取请求时使用的域名地址
		/// 例 http://localhost 后面不带/
		/// </summary>
		/// <returns></returns>
		public static string GetRequestHostUrl() {
			var context = HttpContext.Current;
			if (context == null) {
				return "http://localhost";
			}
			return context.Request.Url.GetLeftPart(UriPartial.Authority);
		}

		/// <summary>
		/// 获取Cookie值
		/// </summary>
		public static string GetCookie(string plugin, string name) {
			// HttpContext.Current等于null时使用备用Cookies储存
			var context = HttpContext.Current;
			var key = $"{plugin}.{name}";
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
		/// </summary>
		public static bool PutCookie(string plugin, string name, string value,
			DateTime? expired = default(DateTime?), bool httpOnly = false) {
			// HttpContext.Current等于null时使用备用Cookies储存
			var context = HttpContext.Current;
			var key = $"{plugin}.{name}";
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
		/// </summary>
		public static bool RemoveCookie(string plugin, string name) {
			return PutCookie(plugin, name, "", new DateTime(1970, 1, 1));
		}
	}
}
