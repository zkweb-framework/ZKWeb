using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Utils.Functions {
	/// <summary>
	/// Http设备的工具类
	/// </summary>
	public static class HttpDeviceUtils {
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
		/// <returns></returns>
		public static DeviceTypes GetClientDevice() {
			// 从缓存获取
			var device = HttpContextUtils.GetData<object>(DeviceKey);
			if (device != null) {
				return (DeviceTypes)device;
			}
			// 先从Cookies再从UserAgent获取
			var deviceFromCookies = HttpContextUtils.GetCookie(DeviceKey);
			if (!string.IsNullOrEmpty(deviceFromCookies)) {
				device = deviceFromCookies.ConvertOrDefault<DeviceTypes>();
			} else {
				var userAgent = HttpContextUtils.CurrentContext?.Request?.UserAgent ?? "";
				device = MobileCheckRegex.IsMatch(userAgent) ? DeviceTypes.Mobile : DeviceTypes.Desktop;
			}
			// 保存到缓存并返回
			HttpContextUtils.PutData(DeviceKey, device);
			return (DeviceTypes)device;
		}

		/// <summary>
		/// 设置客户端的设备类型到Cookies中
		/// 重新打开浏览器之前都不需要再设置
		/// </summary>
		/// <param name="type">设备类型</param>
		public static void SetClientDeviceToCookies(DeviceTypes type) {
			HttpContextUtils.PutCookie(DeviceKey, type.ToString());
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
}
