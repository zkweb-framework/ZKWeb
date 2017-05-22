using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Locale utility languages<br/>
	/// <br/>
	/// </summary>
	public static class LocaleUtils {
		/// <summary>
		/// The key for store using language code<br/>
		/// <br/>
		/// </summary>
		public const string LanguageKey = "ZKWeb.Language";
		/// <summary>
		/// The key for store using timezone<br/>
		/// <br/>
		/// </summary>
		public const string TimeZoneKey = "ZKWeb.TimeZone";
		/// <summary>
		/// Cache for cluture information<br/>
		/// <br/>
		/// </summary>
		private static ConcurrentDictionary<string, CultureInfo> ClutureInfoCache =
			new ConcurrentDictionary<string, CultureInfo>();
		/// <summary>
		/// Cache for timezone<br/>
		/// <br/>
		/// </summary>
		private static ConcurrentDictionary<string, TimeZoneInfo> TimeZoneInfoCache =
			new ConcurrentDictionary<string, TimeZoneInfo>();
		/// <summary>
		/// Olson to windows timezone mapping<br/>
		/// <br/>
		/// http://stackoverflow.com/questions/5996320/net-timezoneinfo-from-olson-time-zone
		/// </summary>
		private static IDictionary<string, string> OlsonToWindowsTimezoneMapping =
			new Dictionary<string, string>() {
				{ "Africa/Bangui", "W. Central Africa Standard Time" },
				{ "Africa/Cairo", "Egypt Standard Time" },
				{ "Africa/Casablanca", "Morocco Standard Time" },
				{ "Africa/Harare", "South Africa Standard Time" },
				{ "Africa/Johannesburg", "South Africa Standard Time" },
				{ "Africa/Lagos", "W. Central Africa Standard Time" },
				{ "Africa/Monrovia", "Greenwich Standard Time" },
				{ "Africa/Nairobi", "E. Africa Standard Time" },
				{ "Africa/Windhoek", "Namibia Standard Time" },
				{ "America/Anchorage", "Alaskan Standard Time" },
				{ "America/Argentina/San_Juan", "Argentina Standard Time" },
				{ "America/Asuncion", "Paraguay Standard Time" },
				{ "America/Bahia", "Bahia Standard Time" },
				{ "America/Bogota", "SA Pacific Standard Time" },
				{ "America/Buenos_Aires", "Argentina Standard Time" },
				{ "America/Caracas", "Venezuela Standard Time" },
				{ "America/Cayenne", "SA Eastern Standard Time" },
				{ "America/Chicago", "Central Standard Time" },
				{ "America/Chihuahua", "Mountain Standard Time (Mexico)" },
				{ "America/Cuiaba", "Central Brazilian Standard Time" },
				{ "America/Denver", "Mountain Standard Time" },
				{ "America/Fortaleza", "SA Eastern Standard Time" },
				{ "America/Godthab", "Greenland Standard Time" },
				{ "America/Guatemala", "Central America Standard Time" },
				{ "America/Halifax", "Atlantic Standard Time" },
				{ "America/Indianapolis", "US Eastern Standard Time" },
				{ "America/Indiana/Indianapolis", "US Eastern Standard Time" },
				{ "America/La_Paz", "SA Western Standard Time" },
				{ "America/Los_Angeles", "Pacific Standard Time" },
				{ "America/Mexico_City", "Mexico Standard Time" },
				{ "America/Montevideo", "Montevideo Standard Time" },
				{ "America/New_York", "Eastern Standard Time" },
				{ "America/Noronha", "UTC-02" },
				{ "America/Phoenix", "US Mountain Standard Time" },
				{ "America/Regina", "Canada Central Standard Time" },
				{ "America/Santa_Isabel", "Pacific Standard Time (Mexico)" },
				{ "America/Santiago", "Pacific SA Standard Time" },
				{ "America/Sao_Paulo", "E. South America Standard Time" },
				{ "America/St_Johns", "Newfoundland Standard Time" },
				{ "America/Tijuana", "Pacific Standard Time" },
				{ "Antarctica/McMurdo", "New Zealand Standard Time" },
				{ "Atlantic/South_Georgia", "UTC-02" },
				{ "Asia/Almaty", "Central Asia Standard Time" },
				{ "Asia/Amman", "Jordan Standard Time" },
				{ "Asia/Baghdad", "Arabic Standard Time" },
				{ "Asia/Baku", "Azerbaijan Standard Time" },
				{ "Asia/Bangkok", "SE Asia Standard Time" },
				{ "Asia/Beirut", "Middle East Standard Time" },
				{ "Asia/Calcutta", "India Standard Time" },
				{ "Asia/Colombo", "Sri Lanka Standard Time" },
				{ "Asia/Damascus", "Syria Standard Time" },
				{ "Asia/Dhaka", "Bangladesh Standard Time" },
				{ "Asia/Dubai", "Arabian Standard Time" },
				{ "Asia/Irkutsk", "North Asia East Standard Time" },
				{ "Asia/Jerusalem", "Israel Standard Time" },
				{ "Asia/Kabul", "Afghanistan Standard Time" },
				{ "Asia/Kamchatka", "Kamchatka Standard Time" },
				{ "Asia/Karachi", "Pakistan Standard Time" },
				{ "Asia/Katmandu", "Nepal Standard Time" },
				{ "Asia/Kolkata", "India Standard Time" },
				{ "Asia/Krasnoyarsk", "North Asia Standard Time" },
				{ "Asia/Kuala_Lumpur", "Singapore Standard Time" },
				{ "Asia/Kuwait", "Arab Standard Time" },
				{ "Asia/Magadan", "Magadan Standard Time" },
				{ "Asia/Muscat", "Arabian Standard Time" },
				{ "Asia/Novosibirsk", "N. Central Asia Standard Time" },
				{ "Asia/Oral", "West Asia Standard Time" },
				{ "Asia/Rangoon", "Myanmar Standard Time" },
				{ "Asia/Riyadh", "Arab Standard Time" },
				{ "Asia/Seoul", "Korea Standard Time" },
				{ "Asia/Shanghai", "China Standard Time" },
				{ "Asia/Singapore", "Singapore Standard Time" },
				{ "Asia/Taipei", "Taipei Standard Time" },
				{ "Asia/Tashkent", "West Asia Standard Time" },
				{ "Asia/Tbilisi", "Georgian Standard Time" },
				{ "Asia/Tehran", "Iran Standard Time" },
				{ "Asia/Tokyo", "Tokyo Standard Time" },
				{ "Asia/Ulaanbaatar", "Ulaanbaatar Standard Time" },
				{ "Asia/Vladivostok", "Vladivostok Standard Time" },
				{ "Asia/Yakutsk", "Yakutsk Standard Time" },
				{ "Asia/Yekaterinburg", "Ekaterinburg Standard Time" },
				{ "Asia/Yerevan", "Armenian Standard Time" },
				{ "Atlantic/Azores", "Azores Standard Time" },
				{ "Atlantic/Cape_Verde", "Cape Verde Standard Time" },
				{ "Atlantic/Reykjavik", "Greenwich Standard Time" },
				{ "Australia/Adelaide", "Cen. Australia Standard Time" },
				{ "Australia/Brisbane", "E. Australia Standard Time" },
				{ "Australia/Darwin", "AUS Central Standard Time" },
				{ "Australia/Hobart", "Tasmania Standard Time" },
				{ "Australia/Perth", "W. Australia Standard Time" },
				{ "Australia/Sydney", "AUS Eastern Standard Time" },
				{ "Etc/GMT", "UTC" },
				{ "Etc/GMT+11", "UTC-11" },
				{ "Etc/GMT+12", "Dateline Standard Time" },
				{ "Etc/GMT+2", "UTC-02" },
				{ "Etc/GMT-12", "UTC+12" },
				{ "Europe/Amsterdam", "W. Europe Standard Time" },
				{ "Europe/Athens", "GTB Standard Time" },
				{ "Europe/Belgrade", "Central Europe Standard Time" },
				{ "Europe/Berlin", "W. Europe Standard Time" },
				{ "Europe/Brussels", "Romance Standard Time" },
				{ "Europe/Budapest", "Central Europe Standard Time" },
				{ "Europe/Dublin", "GMT Standard Time" },
				{ "Europe/Helsinki", "FLE Standard Time" },
				{ "Europe/Istanbul", "GTB Standard Time" },
				{ "Europe/Kiev", "FLE Standard Time" },
				{ "Europe/London", "GMT Standard Time" },
				{ "Europe/Minsk", "E. Europe Standard Time" },
				{ "Europe/Moscow", "Russian Standard Time" },
				{ "Europe/Paris", "Romance Standard Time" },
				{ "Europe/Sarajevo", "Central European Standard Time" },
				{ "Europe/Warsaw", "Central European Standard Time" },
				{ "Indian/Mauritius", "Mauritius Standard Time" },
				{ "Pacific/Apia", "Samoa Standard Time" },
				{ "Pacific/Auckland", "New Zealand Standard Time" },
				{ "Pacific/Fiji", "Fiji Standard Time" },
				{ "Pacific/Guadalcanal", "Central Pacific Standard Time" },
				{ "Pacific/Guam", "West Pacific Standard Time" },
				{ "Pacific/Honolulu", "Hawaiian Standard Time" },
				{ "Pacific/Pago_Pago", "UTC-11" },
				{ "Pacific/Port_Moresby", "West Pacific Standard Time" },
				{ "Pacific/Tongatapu", "Tonga Standard Time" }
			};
		/// <summary>
		/// Windows to olson timezone mapping<br/>
		/// <br/>
		/// </summary>
		private static IDictionary<string, string> WindowsToOlsonTimezoneMapping =
			OlsonToWindowsTimezoneMapping.GroupBy(p => p.Value).ToDictionary(g => g.Key, g => g.First().Key);

		/// <summary>
		/// Get timezone information<br/>
		/// Support use windows timezone name on linux, or use linux timezone name on windows<br/>
		/// Return null if not found<br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="timezone">Timezone name</param>
		/// <returns></returns>
		public static TimeZoneInfo GetTimezoneInfo(string timezone) {
			// Try original
			try {
				var timezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezone);
				if (timezoneInfo != null) {
					return timezoneInfo;
				}
			} catch (Exception) {
				// TimeZoneNotFoundException is not available
				// https://github.com/dotnet/corefx/blob/master/src/System.Runtime/tests/System/TimeZoneInfoTests.cs
			}
			// Try converted
			timezone = WindowsToOlsonTimezoneMapping.GetOrDefault(timezone) ??
				OlsonToWindowsTimezoneMapping.GetOrDefault(timezone);
			try {
				var timezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezone);
				if (timezoneInfo != null) {
					return timezoneInfo;
				}
			} catch (Exception) { }
			// Not found
			return null;
		}

		/// <summary>
		/// Get culture information<br/>
		/// Return null if not found<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="language">Language code</param>
		/// <returns></returns>
		public static CultureInfo GetCultureInfo(string language) {
			try {
				var cultureInfo = new CultureInfo(language);
				// On linux .Net Core will return a pesudo culture info for language does not exist
				// There no good way to detect it
				if (cultureInfo.TwoLetterISOLanguageName.Length != 2 && cultureInfo.IsNeutralCulture) {
					return null;
				}
				return cultureInfo;
			} catch (CultureNotFoundException) {
				return null;
			}
		}

		/// <summary>
		/// Set using languange code, return true for success<br/>
		/// <br/>
		/// </summary>
		/// <param name="language">Language code</param>
		/// <returns></returns>
		public static bool SetThreadLanguage(string language) {
			if (string.IsNullOrEmpty(language)) {
				return false;
			}
			var cultureInfo = ClutureInfoCache.GetOrAdd(language, key => GetCultureInfo(key));
			if (cultureInfo != null) {
				CultureInfo.CurrentCulture = cultureInfo;
				CultureInfo.CurrentUICulture = cultureInfo;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Set using timezone, return true for success<br/>
		/// <br/>
		/// </summary>
		/// <param name="timezone">Timezone name</param>
		/// <returns></returns>
		public static bool SetThreadTimezone(string timezone) {
			if (string.IsNullOrEmpty(timezone)) {
				return false;
			}
			var timezoneInfo = TimeZoneInfoCache.GetOrAdd(timezone, key => GetTimezoneInfo(key));
			if (timezoneInfo != null) {
				HttpManager.CurrentContext.PutData(TimeZoneKey, timezoneInfo);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Automatic set using language name, return true for success<br/>
		/// Flow<br/>
		/// - Use language code from cookie<br/>
		/// - Use accept languages from client, if allowed<br/>
		/// - Use default language<br/>
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
		/// Automatic set using timezone, return true for success<br/>
		/// Flow<br/>
		/// - Use timezone name from cookies<br/>
		/// - Use default timezone<br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// <br/>
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
