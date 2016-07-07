using System;
using System.Globalization;
using System.Threading;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

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
			var language = CultureInfo.CurrentCulture.Name;
			var timezone = HttpManager.CurrentContext.GetData<TimeZoneInfo>(LocaleUtils.TimeZoneKey);
			return Pair.Create(language, timezone);
		}
	}
}
