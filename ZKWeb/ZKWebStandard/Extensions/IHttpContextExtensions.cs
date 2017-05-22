using System;
using System.Text.RegularExpressions;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// IHttpContext extension methods<br/>
	/// Http上下文的扩展函数<br/>
	/// </summary>
	public static class IHttpContextExtensions {
		/// <summary>
		/// Put associated data to http context<br/>
		/// 插入Http上下文的关联数据<br/>
		/// </summary>
		/// <typeparam name="T">Data type</typeparam>
		/// <param name="context">Http context</param>
		/// <param name="key">Key</param>
		/// <param name="data">Data</param>
		/// <example>
		/// <code language="cs">
		/// var list = new string[] { "a", "b", "c" };
		/// HttpManager.CurrentContext.PutData("TestPutData", list);
		/// </code>
		/// </example>
		public static void PutData<T>(this IHttpContext context, string key, T data) {
			context.Items[key] = data;
		}

		/// <summary>
		/// Get associated data from http context<br/>
		/// 获取Http上下文的关联数据<br/>
		/// </summary>
		/// <typeparam name="T">Data type</typeparam>
		/// <param name="context">Http context</param>
		/// <param name="key">Key</param>
		/// <param name="defaultValue">The default value</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var result = HttpManager.CurrentContext.GetData&lt;string[]&gt;("TestPutData");
		/// </code>
		/// </example>
		public static T GetData<T>(
			this IHttpContext context, string key, T defaultValue = default(T)) {
			var obj = context.Items.GetOrDefault(key);
			if (obj is T) {
				return (T)obj;
			}
			return defaultValue;
		}

		/// <summary>
		/// Get associated data from http context, create it first if not exist<br/>
		/// 获取Http上下文的关联数据, 如果不存在则创建<br/>
		/// </summary>
		/// <typeparam name="T">Data type</typeparam>
		/// <param name="context">Http context</param>
		/// <param name="key">Key</param>
		/// <param name="defaultCreater">Default value factory</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var result = HttpManager.CurrentContext.GetOrCreateData("TestCreateData", () =&gt; "def");
		/// </code>
		/// </example>
		public static T GetOrCreateData<T>(
			this IHttpContext context, string key, Func<T> defaultCreater) {
			var value = context.GetData<T>(key);
			if (value == null) {
				value = defaultCreater();
				context.PutData(key, value);
			}
			return value;
		}

		/// <summary>
		/// Remove associated data from http context<br/>
		/// 删除Http上下文的关联数据<br/>
		/// </summary>
		/// <param name="context">Http context</param>
		/// <param name="key">Key</param>
		/// <example>
		/// <code language="cs">
		/// HttpManager.CurrentContext.RemoveData("TestRemoveData");
		/// </code>
		/// </example>
		public static void RemoveData(
			this IHttpContext context, string key) {
			context.Items.Remove(key);
		}

		/// <summary>
		/// Data key prefix for previous set cookie value<br/>
		/// If we write a cookie value and read it later in the same http context,<br/>
		/// we will need to know what value is wrote before<br/>
		/// 设置Cookie时使用的关联键的前缀<br/>
		/// 如果我们写了一个Cookie值然后又想在Http上下文结束前读取它,<br/>
		/// 我们需要知道之前写入了什么值<br/>
		/// </summary>
		public const string SetCookieDataKeyPrefix = "__ZKWeb.SetCookie.";

		/// <summary>
		/// Get cookie value from http context<br/>
		/// 获取Http上下文中的Cookie值<br/>
		/// </summary>
		/// <param name="context">Http context</param>
		/// <param name="key">Cookie key</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var cookie = HttpManager.CurrentContext.GetCookie("TestGetCookie");
		/// </code>
		/// </example>
		public static string GetCookie(
			this IHttpContext context, string key) {
			// Check if this cookie is set before
			// If so we need to use the previous value
			var dataKey = SetCookieDataKeyPrefix + key;
			var cookie = context.GetData<string>(dataKey);
			if (cookie != null) {
				return cookie;
			}
			// Get cookie value from http request
			cookie = context.Request.GetCookie(key);
			return HttpUtils.UrlDecode(cookie);
		}

		/// <summary>
		/// Put cookie value to http context<br/>
		/// 设置Http上下文中的Cookie值<br/>
		/// </summary>
		/// <param name="context">Http context</param>
		/// <param name="key">Cookie key</param>
		/// <param name="value">Cookie value</param>
		/// <param name="options">Cookie options</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// HttpManager.CurrentContext.PutCookie("TestPutCookie", "abc");
		/// </code>
		/// </example>
		public static void PutCookie(
			this IHttpContext context, string key, string value, HttpCookieOptions options = null) {
			// Record the value to http context
			var dataKey = SetCookieDataKeyPrefix + key;
			context.PutData(dataKey, value);
			// Set cookie value to http response
			var cookie = HttpUtils.UrlEncode(value);
			context.Response.SetCookie(key, cookie, options ?? new HttpCookieOptions());
		}

		/// <summary>
		/// Remove cookie value<br/>
		/// 删除Cookie值<br/>
		/// </summary>
		/// <param name="context">Http context</param>
		/// <param name="key">Cookie key</param>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// HttpManager.CurrentContext.RemoveCookie("TestRemoveCookie");
		/// </code>
		/// </example>
		public static void RemoveCookie(this IHttpContext context, string key) {
			var options = new HttpCookieOptions() { Expires = new DateTime(1970, 1, 1) };
			context.PutCookie(key, "", options);
		}

		/// <summary>
		/// Determines client is visiting from mobile device<br/>
		/// Ignore some device for performance<br/>
		/// 检测客户端是否移动端设备<br/>
		/// 为了性能忽略部分设备<br/>
		/// See: http://stackoverflow.com/questions/13086856/mobile-device-detection-in-asp-net
		/// </summary>
		private static Regex MobileCheckRegex = new Regex(
			@"android|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|" +
			@"ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|" +
			@"phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|" +
			@"windows (ce|phone)|xda|xiino",
			RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
		/// <summary>
		/// Client device key name, used for<br/>
		/// - Cache the device type for performance<br/>
		/// - Get from cookie if client want to provide a specified device type<br/>
		/// 客户端设备的储存键, 用于<br/>
		/// - 为了性能缓存检测出来的设备类型<br/>
		/// - 从Cookie获取如果客户端想手动指定设备类型<br/>
		/// </summary>
		public const string DeviceKey = "ZKWeb.Device";

		/// <summary>
		/// Get client device type<br/>
		/// From Cache => Cookie => User-Agent<br/>
		/// 获取客户端的设备类型<br/>
		/// 顺序 缓存 => Cookie值 => User-Agent<br/>
		/// </summary>
		/// <param name="context">Http context</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var device = HttpManager.CurrentContext.GetClientDevice();
		/// </code>
		/// </example>
		public static DeviceTypes GetClientDevice(this IHttpContext context) {
			var device = context.GetData<object>(DeviceKey);
			if (device != null) {
				return (DeviceTypes)device;
			}
			var deviceFromCookies = context.GetCookie(DeviceKey);
			if (!string.IsNullOrEmpty(deviceFromCookies)) {
				device = deviceFromCookies.ConvertOrDefault<DeviceTypes>();
			} else {
				var userAgent = context.Request.GetUserAgent() ?? "";
				device = MobileCheckRegex.IsMatch(userAgent) ? DeviceTypes.Mobile : DeviceTypes.Desktop;
			}
			context.PutData(DeviceKey, device);
			return (DeviceTypes)device;
		}

		/// <summary>
		/// Set device type to cookies<br/>
		/// Next time will use this value instead of the value detect from user agent<br/>
		/// 设置客户端的设备类型<br/>
		/// 下次请求将会使用这个值代替User-Agent中的值<br/>
		/// </summary>
		/// <param name="context">Http ccontext</param>
		/// <param name="type">Device type</param>
		/// <example>
		/// <code language="cs">
		/// HttpManager.CurrentContext.SetClientDeviceToCookies(DeviceTypes.Desktop);
		/// </code>
		/// </example>
		public static void SetClientDeviceToCookies(this IHttpContext context, DeviceTypes type) {
			context.PutCookie(DeviceKey, type.ToString());
		}
	}

	/// <summary>
	/// Device type<br/>
	/// 设备类型<br/>
	/// </summary>
	public enum DeviceTypes {
		/// <summary>
		/// Desktop<br/>
		/// PC端<br/>
		/// </summary>
		Desktop = 0,
		/// <summary>
		/// Mobile<br/>
		/// 移动端<br/>
		/// </summary>
		Mobile = 1
	}
}
