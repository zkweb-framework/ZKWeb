using System;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Date time extension methods
	/// </summary>
	public static class DateTimeExtensions {
		/// <summary>
		/// Convert UTC time to client time, use the timezone from client
		/// </summary>
		/// <param name="time">UTC time</param>
		/// <returns></returns>
		public static DateTime ToClientTime(this DateTime time) {
			// Get timezone from client
			// If timezone exist, use it to convert time
			var timezone = HttpManager.CurrentContext.GetData<TimeZoneInfo>(LocaleUtils.TimeZoneKey);
			if (timezone != null) {
				time = DateTime.SpecifyKind(time, DateTimeKind.Utc);
				return TimeZoneInfo.ConvertTime(time, TimeZoneInfo.Utc, timezone);
			}
			// If timezone not exist, use server local time
			return time.ToLocalTime();
		}

		/// <summary>
		/// Convert UTC time to client time as string
		/// Format is yyyy/MM/dd HH:mm:ss
		/// </summary>
		/// <param name="time">UTC time</param>
		/// <returns></returns>
		public static string ToClientTimeString(this DateTime time) {
			return time.ToClientTime().ToString("yyyy/MM/dd HH:mm:ss");
		}

		/// <summary>
		/// Convert client time to UTC time, use the timezone from client
		/// </summary>
		/// <param name="time">本地时间</param>
		/// <returns></returns>
		public static DateTime FromClientTime(this DateTime time) {
			// Get timezone from client
			// If timezone exist, use it to convert time
			var timezone = HttpManager.CurrentContext.GetData<TimeZoneInfo>(LocaleUtils.TimeZoneKey);
			if (timezone != null) {
				time = DateTime.SpecifyKind(time, DateTimeKind.Unspecified);
				return TimeZoneInfo.ConvertTime(time, timezone, TimeZoneInfo.Utc);
			}
			// If timezone not exist, assume it's server local time
			return time.ToUniversalTime();
		}

		/// <summary>
		/// Truncate datetime, only keep seconds
		/// </summary>
		/// <param name="time">The time</param>
		/// <returns></returns>
		public static DateTime Truncate(this DateTime time) {
			return time.AddTicks(-(time.Ticks % TimeSpan.TicksPerSecond));
		}

		/// <summary>
		/// Return unix style timestamp
		/// Return a minus value if the time early than 1970-1-1
		/// The given time will be converted to UTC time first
		/// </summary>
		/// <param name="time">The time</param>
		/// <returns></returns>
		public static TimeSpan ToTimestamp(this DateTime time) {
			return time.ToUniversalTime() - new DateTime(1970, 1, 1);
		}
	}
}
