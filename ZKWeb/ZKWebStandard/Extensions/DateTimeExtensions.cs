using System;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Date time extension methods<br/>
	/// 时间的扩展函数<br/>
	/// </summary>
	public static class DateTimeExtensions {
		/// <summary>
		/// Convert UTC time to client time, use the timezone from client<br/>
		/// 转换UTC时间到客户端的本地时间, 使用客户端提供的时区<br/>
		/// </summary>
		/// <param name="time">UTC time</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// HttpManager.CurrentContext.RemoveData(LocaleUtils.TimeZoneKey);
		/// var time = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		/// var clientTime = time.ToClientTime(); // == time.ToLocalTime()
		/// 
		/// LocaleUtils.SetThreadTimezone("Asia/Shanghai");
		/// clientTime = time.ToClientTime(); // == new DateTime(2000, 1, 1, 8, 0, 0, DateTimeKind.Local)
		/// 
		///	LocaleUtils.SetThreadTimezone("Etc/GMT");
		///	clientTime = time.ToClientTime(); // == new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local)
		/// </code>
		/// </example>
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
		/// Convert UTC time to client time as string<br/>
		/// Format is yyyy/MM/dd HH:mm:ss<br/>
		/// 转换UTC时间到客户端的本地时间字符串<br/>
		/// 格式是 yyyy/MM/dd HH:mm:ss<br/>
		/// </summary>
		/// <param name="time">UTC time</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// HttpManager.CurrentContext.RemoveData(LocaleUtils.TimeZoneKey);
		/// LocaleUtils.SetThreadTimezone("Asia/Shanghai");
		/// 
		///	var time = new DateTime(2000, 1, 2, 13, 14, 50, DateTimeKind.Utc);
		/// var timeString = time.ToClientTimeString(); // "2000/01/02 21:14:50"
		/// </code>
		/// </example>
		public static string ToClientTimeString(this DateTime time) {
			return time.ToClientTime().ToString("yyyy/MM/dd HH:mm:ss");
		}

		/// <summary>
		/// Convert client time to UTC time, use the timezone from client<br/>
		/// 转换客户端的本地时间到UTC时间， 使用客户端提供的时区<br/>
		/// </summary>
		/// <param name="time">Client time</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// HttpManager.CurrentContext.RemoveData(LocaleUtils.TimeZoneKey);
		/// var time = new DateTime(2000, 1, 1, 8, 0, 0, DateTimeKind.Local);
		/// var utcTime = time.FromClientTime(); // == time.ToUniversalTime()
		/// 
		///	LocaleUtils.SetThreadTimezone("Asia/Shanghai");
		///	utcTime = time.FromClientTime(); // == new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
		/// 
		///	LocaleUtils.SetThreadTimezone("Etc/GMT");
		///	utcTime = time.FromClientTime(); // == new DateTime(2000, 1, 1, 8, 0, 0, DateTimeKind.Utc)
		/// </code>
		/// </example>
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
		/// Truncate datetime, only keep seconds<br/>
		/// 去掉时间中秒数后的部分<br/>
		/// </summary>
		/// <param name="time">The time</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var time = DateTime.UtcNow;
		/// var truncated = time.Truncate();
		/// </code>
		/// </example>
		public static DateTime Truncate(this DateTime time) {
			return time.AddTicks(-(time.Ticks % TimeSpan.TicksPerSecond));
		}

		/// <summary>
		/// Return unix style timestamp<br/>
		/// Return a minus value if the time early than 1970-1-1<br/>
		/// The given time will be converted to UTC time first<br/>
		/// 获取unix风格的时间戳<br/>
		/// 如果时间早于1970年1月1日则返回值是负数<br/>
		/// 参数会先转换为UTC时间<br/>
		/// </summary>
		/// <param name="time">The time</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var time = new DateTime(1970, 1, 2, 0, 0, 0, DateTimeKind.Utc);
		/// var timeStamp = (int)time.ToTimestamp().TotalSeconds; // == 86400
		/// </code>
		/// </example>
		public static TimeSpan ToTimestamp(this DateTime time) {
			return time.ToUniversalTime() - new DateTime(1970, 1, 1);
		}
	}
}
