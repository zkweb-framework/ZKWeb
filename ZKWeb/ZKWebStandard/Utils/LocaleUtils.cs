using System;
using System.Collections.Concurrent;
using System.Globalization;
using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Locale utility languages
	/// </summary>
	public static class LocaleUtils {
		/// <summary>
		/// The key for store using language code
		/// </summary>
		public const string LanguageKey = "ZKWeb.Language";
		/// <summary>
		/// The key for store using timezone
		/// </summary>
		public const string TimeZoneKey = "ZKWeb.TimeZone";
		/// <summary>
		/// Cache for cluture information
		/// </summary>
		private static ConcurrentDictionary<string, CultureInfo> ClutureInfoCache =
			new ConcurrentDictionary<string, CultureInfo>();
		/// <summary>
		/// Cache for timezone
		/// </summary>
		private static ConcurrentDictionary<string, TimeZoneInfo> TimeZoneInfoCache =
			new ConcurrentDictionary<string, TimeZoneInfo>();

		/// <summary>
		/// Set using languange code, return true for success
		/// </summary>
		/// <param name="language">Language code</param>
		/// <returns></returns>
		public static bool SetThreadLanguage(string language) {
			if (string.IsNullOrEmpty(language)) {
				return false;
			}
			try {
				var cultureInfo = ClutureInfoCache.GetOrAdd(
					language, key => new CultureInfo(key));
				CultureInfo.CurrentCulture = cultureInfo;
				CultureInfo.CurrentUICulture = cultureInfo;
				return true;
			} catch (CultureNotFoundException) {
				return false;
			}
		}

		/// <summary>
		/// Set using timezone, return true for success
		/// </summary>
		/// <param name="timezone">Timezone name</param>
		/// <returns></returns>
		public static bool SetThreadTimezone(string timezone) {
			if (string.IsNullOrEmpty(timezone)) {
				return false;
			}
			try {
				var timezoneInfo = TimeZoneInfoCache.GetOrAdd(
					timezone, key => TimeZoneInfo.FindSystemTimeZoneById(key));
				HttpManager.CurrentContext.PutData(TimeZoneKey, timezoneInfo);
				return true;
			} catch (Exception) {
				// TimeZoneNotFoundException is not available
				// https://github.com/dotnet/corefx/blob/master/src/System.Runtime/tests/System/TimeZoneInfoTests.cs
				return false;
			}
		}

		/// <summary>
		/// Automatic set using language name, return true for success
		/// Flow
		/// - Use language code from cookie
		/// - Use accept languages from client, if allowed
		/// - Use default language
		/// </summary>
		/// <param name="allowDetectLanguageFromBrowser">Allow use accept languages from client</param>
		/// <param name="defaultLanguage">Default language</param>
		/// <returns></returns>
		public static bool SetThreadLanguageAutomatic(
			bool allowDetectLanguageFromBrowser, string defaultLanguage) {
			// Use language code from cookie
			var context = HttpManager.CurrentContext;
			var languageFromCookies = context.GetCookie(LanguageKey);
			if (SetThreadLanguage(languageFromCookies)) {
				return true;
			}
			// Use accept languages from client, if allowed
			if (allowDetectLanguageFromBrowser) {
				var userLanguages = context.Request.GetAcceptLanguages();
				foreach (var languageFromBrowser in userLanguages) {
					if (SetThreadLanguage(languageFromBrowser)) {
						return true;
					}
				}
			}
			// Use default language
			return SetThreadLanguage(defaultLanguage);
		}

		/// <summary>
		/// Automatic set using timezone, return true for success
		/// Flow
		/// - Use timezone name from cookies
		/// - Use default timezone
		/// </summary>
		/// <param name="defaultTimezone">Default timezone</param>
		/// <returns></returns>
		public static bool SetThreadTimezoneAutomatic(string defaultTimezone) {
			// Use timezone name from cookies
			var context = HttpManager.CurrentContext;
			var timezoneFromCookies = context.GetCookie(TimeZoneKey);
			if (SetThreadTimezone(timezoneFromCookies)) {
				return true;
			}
			// Use default timezone
			return SetThreadTimezone(defaultTimezone);
		}
	}
}
