using System;
using System.Text.RegularExpressions;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// IHttpContext extension methods
	/// </summary>
	public static class IHttpContextExtensions {
		/// <summary>
		/// Put associated data to http context
		/// </summary>
		/// <typeparam name="T">Data type</typeparam>
		/// <param name="context">Http context</param>
		/// <param name="key">Key</param>
		/// <param name="data">Data</param>
		public static void PutData<T>(this IHttpContext context, string key, T data) {
			context.Items[key] = data;
		}

		/// <summary>
		/// Get associated data from http context
		/// </summary>
		/// <typeparam name="T">Data type</typeparam>
		/// <param name="context">Http context</param>
		/// <param name="key">Key</param>
		/// <param name="defaultValue">The default value</param>
		/// <returns></returns>
		public static T GetData<T>(
			this IHttpContext context, string key, T defaultValue = default(T)) {
			var obj = context.Items.GetOrDefault(key);
			if (obj is T) {
				return (T)obj;
			}
			return defaultValue;
		}

		/// <summary>
		/// Get associated data from http context, create it first if not exist
		/// </summary>
		/// <typeparam name="T">Data type</typeparam>
		/// <param name="context">Http context</param>
		/// <param name="key">Key</param>
		/// <param name="defaultCreater">Default value factory</param>
		/// <returns></returns>
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
		/// Remove associated data from http context
		/// </summary>
		/// <param name="context">Http context</param>
		/// <param name="key">Key</param>
		public static void RemoveData(
			this IHttpContext context, string key) {
			context.Items.Remove(key);
		}

		/// <summary>
		/// Data key prefix for previous set cookie value
		/// If we write a cookie value and read it later in the same http context,
		/// we will need to know what value is wrote before
		/// </summary>
		public const string SetCookieDataKeyPrefix = "__ZKWeb.SetCookie.";

		/// <summary>
		/// Get cookie value from http context
		/// </summary>
		/// <param name="context">Http context</param>
		/// <param name="key">Cookie key</param>
		/// <returns></returns>
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
		/// Put cookie value to http context
		/// </summary>
		/// <param name="context">Http context</param>
		/// <param name="key">Cookie key</param>
		/// <param name="value">Cookie value</param>
		/// <param name="options">Cookie options</param>
		/// <returns></returns>
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
		/// Remove cookie value
		/// </summary>
		/// <param name="context">Http context</param>
		/// <param name="key">Cookie key</param>
		/// <returns></returns>
		public static void RemoveCookie(this IHttpContext context, string key) {
			var options = new HttpCookieOptions() { Expires = new DateTime(1970, 1, 1) };
			context.PutCookie(key, "", options);
		}

		/// <summary>
		/// Determines client is visiting from mobile device
		/// Ignore some device for performance
		/// See: http://stackoverflow.com/questions/13086856/mobile-device-detection-in-asp-net
		/// </summary>
		private static Regex MobileCheckRegex = new Regex(
			@"android|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|" +
			@"ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|" +
			@"phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|" +
			@"windows (ce|phone)|xda|xiino",
			RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
		/// <summary>
		/// Client device key name, used for
		/// - Cache the device type for performance
		/// - Get from cookie if client want to provide a specified device type 
		/// </summary>
		public const string DeviceKey = "ZKWeb.Device";

		/// <summary>
		/// Get client device type
		/// From Cache => Cookie => UserAgent
		/// </summary>
		/// <param name="context">Http context</param>
		/// <returns></returns>
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
		/// Set device type to cookies
		/// Next time will use this value instead of the value detect from user agent
		/// </summary>
		/// <param name="context">Http ccontext</param>
		/// <param name="type">Device type</param>
		public static void SetClientDeviceToCookies(this IHttpContext context, DeviceTypes type) {
			context.PutCookie(DeviceKey, type.ToString());
		}
	}

	/// <summary>
	/// Device type
	/// </summary>
	public enum DeviceTypes {
		/// <summary>
		/// Desktop
		/// </summary>
		Desktop = 0,
		/// <summary>
		/// Mobile
		/// </summary>
		Mobile = 1
	}
}
