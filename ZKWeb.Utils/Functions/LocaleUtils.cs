using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ZKWeb.Utils.Functions {
	/// <summary>
	/// 语言和时区的工具类
	/// </summary>
	public static class LocaleUtils {
		/// <summary>
		/// 设置当前线程使用的语言，返回是否成功
		/// </summary>
		/// <param name="language">语言代码</param>
		/// <returns></returns>
		public static bool SetThreadLanguage(string language) {
			if (string.IsNullOrEmpty(language)) {
				return false;
			}
			try {
				var cultureInfo = CultureInfo.GetCultureInfo(language);
				Thread.CurrentThread.CurrentCulture = cultureInfo;
				Thread.CurrentThread.CurrentUICulture = cultureInfo;
				return true;
			} catch (CultureNotFoundException) {
				return false;
			}
		}

		/// <summary>
		/// 设置当前线程使用的时区，返回是否成功
		/// </summary>
		/// <param name="timezone">时区名称</param>
		/// <returns></returns>
		public static bool SetThreadTimezone(string timezone) {
			if (string.IsNullOrEmpty(timezone)) {
				return false;
			}
			try {
				var timezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezone);
				HttpContextUtils.PutData("ZKWeb", "Timezone", timezoneInfo);
				return true;
			} catch (TimeZoneNotFoundException) {
				return false;
			}
		}

		/// <summary>
		/// 自动设置当前线程使用的语言，返回是否成功
		/// 设置顺序
		///		获取Cookies中指定的语言，设置成功时返回
		///		允许检测浏览器语言时使用浏览器指定的语言，设置成功时返回
		///		使用传入的默认语言，返回是否成功
		/// </summary>
		/// <param name="allowDetectLanguageFromBrowser">是否允许检测浏览器语言</param>
		/// <param name="defaultLanguage">默认语言</param>
		/// <returns></returns>
		public static bool SetThreadLanguageAutomatic(
			bool allowDetectLanguageFromBrowser, string defaultLanguage) {
			// 获取Cookies中指定的语言，设置成功时返回
			var languageFromCookies = HttpContextUtils.GetCookie("ZKWeb", "Language");
			if (SetThreadLanguage(languageFromCookies)) {
				return true;
			}
			// 允许检测浏览器语言时使用浏览器指定的语言，设置成功时返回
			var context = HttpContext.Current;
			if (allowDetectLanguageFromBrowser &&
				context != null && context.Request.UserLanguages != null) {
				foreach (var languageFromBrowser in context.Request.UserLanguages) {
					if (SetThreadLanguage(languageFromBrowser)) {
						return true; // 设置成功时返回
					}
				}
			}
			// 使用传入的默认语言，返回是否成功
			return SetThreadLanguage(defaultLanguage);
		}

		/// <summary>
		/// 自动设置当前线程使用的时区，返回是否成功
		/// 设置顺序
		///		获取Cookies中指定的时区，设置成功时返回
		///		使用传入的默认时区，返回是否成功
		/// </summary>
		/// <param name="defaultTimezone">默认时区</param>
		/// <returns></returns>
		public static bool SetThreadTimezoneAutomatic(string defaultTimezone) {
			// 获取Cookies中指定的时区，设置成功时返回
			var timezoneFromCookies = HttpContextUtils.GetCookie("ZKWeb", "Timezone");
			if (SetThreadTimezone(timezoneFromCookies)) {
				return true;
			}
			// 使用传入的默认时区，返回是否成功
			return SetThreadTimezone(defaultTimezone);
		}
	}
}