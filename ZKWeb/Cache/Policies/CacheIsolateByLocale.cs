using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using ZKWeb.Cache.Interfaces;
using ZKWeb.Utils.Collections;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Cache.Policies {
	/// <summary>
	/// 按当前语言和时区隔离缓存
	/// </summary>
	public class CacheIsolateByLocale : ICacheIsolationPolicy {
		/// <summary>
		/// 获取隔离键
		/// </summary>
		/// <returns></returns>
		public object GetIsolationKey() {
			var language = Thread.CurrentThread.CurrentCulture.Name;
			var timezone = HttpContextUtils.GetData<TimeZoneInfo>(LocaleUtils.TimeZoneKey);
			return Pair.Create(language, timezone);
		}
	}
}
