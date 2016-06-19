using System;
using System.Text.RegularExpressions;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Http上下文的接口的扩展函数
	/// </summary>
	public static class IHttpContextExtensions {
		/// <summary>
		/// 储存上下文中通用的数据
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="context">Http上下文</param>
		/// <param name="key">键名</param>
		/// <param name="data">数据</param>
		public static void PutData<T>(this IHttpContext context, string key, T data) {
			context.Items[key] = data;
		}

		/// <summary>
		/// 获取上下文中通用的数据
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="context">Http上下文</param>
		/// <param name="key">键名</param>
		/// <param name="defaultValue">获取不到时返回的默认值</param>
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
		/// 获取上下文中通用的数据，不存在时创建
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="context">Http上下文</param>
		/// <param name="key">键名</param>
		/// <param name="defaultCreater">获取不到时创建默认值使用的函数</param>
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
		/// 删除上下文中通用的数据
		/// </summary>
		/// <param name="context">Http上下文</param>
		/// <param name="key">键名</param>
		public static void RemoveData(
			this IHttpContext context, string key) {
			context.Items.Remove(key);
		}

		/// <summary>
		/// 储存设置的Cookie值时使用的前缀
		/// 如果在获取前设置了需要使用设置的值
		/// </summary>
		public const string SetCookieDataKeyPrefix = "__ZKWeb.SetCookie.";

		/// <summary>
		/// 获取上下文中的Cookie值
		/// </summary>
		/// <param name="context">Http上下文</param>
		/// <param name="key">键值</param>
		/// <returns></returns>
		public static string GetCookie(
			this IHttpContext context, string key) {
			// 如果在获取前设置了需要使用设置的值
			var dataKey = SetCookieDataKeyPrefix + key;
			var cookie = context.GetData<string>(dataKey);
			if (cookie != null) {
				return cookie;
			}
			// 从请求获取值
			cookie = context.Request.GetCookie(key);
			return HttpUtils.UrlDecode(cookie);
		}

		/// <summary>
		/// 设置上下文中的Cookie值
		/// </summary>
		/// <param name="context">Http上下文</param>
		/// <param name="key">键名</param>
		/// <param name="value">Cookie值</param>
		/// <param name="options">使用的选项</param>
		/// <returns></returns>
		public static void PutCookie(
			this IHttpContext context, string key, string value, HttpCookieOptions options = null) {
			// 储存到数据中
			var dataKey = SetCookieDataKeyPrefix + key;
			context.PutData(dataKey, value);
			// 设置Cookie值
			var cookie = HttpUtils.UrlEncode(value);
			context.Response.SetCookie(key, cookie, options ?? new HttpCookieOptions());
		}

		/// <summary>
		/// 删除上下文中的Cookie值
		/// </summary>
		/// <param name="context">Http上下文</param>
		/// <param name="key">键名</param>
		/// <returns></returns>
		public static void RemoveCookie(this IHttpContext context, string key) {
			var options = new HttpCookieOptions() { Expires = new DateTime(1970, 1, 1) };
			context.PutCookie(key, "", options);
		}

		/// <summary>
		/// 检测客户端的UserAgent是否移动端使用的正则表达式
		/// 为了提升速度，不检测部分机型
		/// 参考:
		/// http://stackoverflow.com/questions/13086856/mobile-device-detection-in-asp-net
		/// </summary>
		private static Regex MobileCheckRegex = new Regex(
			@"android|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|" +
			@"ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|" +
			@"phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|" +
			@"windows (ce|phone)|xda|xiino",
			RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
		/// <summary>
		/// 储存当前请求使用设备的键名
		/// 使用这个键保存到Cookies可以手动指定当前请求使用的设备
		/// </summary>
		public const string DeviceKey = "ZKWeb.Device";

		/// <summary>
		/// 获取客户端的设备类型
		/// 先从Cookies再从UserAgent获取
		/// </summary>
		/// <param name="context">Http上下文</param>
		/// <returns></returns>
		public static DeviceTypes GetClientDevice(this IHttpContext context) {
			// 从缓存获取
			var device = context.GetData<object>(DeviceKey);
			if (device != null) {
				return (DeviceTypes)device;
			}
			// 先从Cookies再从UserAgent获取
			var deviceFromCookies = context.GetCookie(DeviceKey);
			if (!string.IsNullOrEmpty(deviceFromCookies)) {
				device = deviceFromCookies.ConvertOrDefault<DeviceTypes>();
			} else {
				var userAgent = context.Request.GetUserAgent() ?? "";
				device = MobileCheckRegex.IsMatch(userAgent) ? DeviceTypes.Mobile : DeviceTypes.Desktop;
			}
			// 保存到缓存并返回
			context.PutData(DeviceKey, device);
			return (DeviceTypes)device;
		}

		/// <summary>
		/// 设置客户端的设备类型到Cookies中
		/// 重新打开浏览器之前都不需要再设置
		/// </summary>
		/// <param name="context">Http上下文</param>
		/// <param name="type">设备类型</param>
		public static void SetClientDeviceToCookies(this IHttpContext context, DeviceTypes type) {
			context.PutCookie(DeviceKey, type.ToString());
		}
	}

	/// <summary>
	/// 设备类型
	/// </summary>
	public enum DeviceTypes {
		/// <summary>
		/// 电脑
		/// </summary>
		Desktop = 0,
		/// <summary>
		/// 手机或平板
		/// </summary>
		Mobile = 1
	}
}
