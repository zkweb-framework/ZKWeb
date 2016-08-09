using System;
using System.Globalization;
using System.Threading;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWeb.Cache.Policies {
	/// <summary>
	/// Isolate cache by culture and timezone
	/// </summary>
	public class CacheIsolateByLocale : ICacheIsolationPolicy {
		/// <summary>
		/// Get isolation key
		/// </summary>
		/// <returns></returns>
		public object GetIsolationKey() {
			var language = CultureInfo.CurrentCulture.Name;
			var timezone = HttpManager.CurrentContext.GetData<TimeZoneInfo>(LocaleUtils.TimeZoneKey);
			return Pair.Create(language, timezone);
		}
	}
}
