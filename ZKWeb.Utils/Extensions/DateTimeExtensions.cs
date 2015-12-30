using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Utils.Extensions {
	/// <summary>
	/// 时间的扩展函数
	/// </summary>
	public static class DateTimeExtensions {

		/// <summary>
		/// 把UTC时间转换到客户端的本地时间
		/// </summary>
		/// <param name="time">utc时间</param>
		/// <returns></returns>
		public static DateTime ToClientTime(this DateTime time) {
			// 获取时区设置，指定了时区时使用时区转换
			var timezone = HttpContextUtils.GetData<TimeZoneInfo>(LocaleUtils.TimeZoneKey);
			if (timezone != null) {
				time = DateTime.SpecifyKind(time, DateTimeKind.Utc);
				return TimeZoneInfo.ConvertTimeFromUtc(time, timezone);
			}
			// 没有时使用服务器的本地时间
			return time.ToLocalTime();
		}

		/// <summary>
		/// 把UTC时间转换到客户端的本地时间字符串
		/// 格式固定是 yyyy/MM/dd HH:mm:ss
		/// 推荐使用这个函数代替 ToClientTime().ToString()，可以不受语言影响
		/// </summary>
		/// <param name="time">utc时间</param>
		/// <returns></returns>
		public static string ToClientTimeString(this DateTime time) {
			return time.ToClientTime().ToString("yyyy/MM/dd HH:mm:ss");
		}

		/// <summary>
		/// 从客户端的本地时间转换到UTC时间
		/// </summary>
		/// <param name="time">本地时间</param>
		/// <returns></returns>
		public static DateTime FromClientTime(this DateTime time) {
			// 获取时区设置，指定了时区时使用时区转换
			var timezone = HttpContextUtils.GetData<TimeZoneInfo>(LocaleUtils.TimeZoneKey);
			if (timezone != null) {
				time = DateTime.SpecifyKind(time, DateTimeKind.Unspecified);
				return TimeZoneInfo.ConvertTimeToUtc(time, timezone);
			}
			// 没有时使用服务器的本地时间
			return time.ToUniversalTime();
		}

		/// <summary>
		/// 只保留时间到秒部分，清空毫秒
		/// </summary>
		/// <param name="time">时间</param>
		/// <returns></returns>
		public static DateTime Truncate(this DateTime time) {
			return time.AddTicks(-(time.Ticks % TimeSpan.TicksPerSecond));
		}

		/// <summary>
		/// 返回Unix格式的时间戳
		/// 时间小于1970-1-1时会返回负值
		/// 传入的时间会使用ToUniversalTime转换成utc时间
		/// </summary>
		/// <param name="time">时间</param>
		/// <returns></returns>
		public static TimeSpan ToTimestamp(this DateTime time) {
			return time.ToUniversalTime() - new DateTime(1970, 1, 1);
		}
	}
}
