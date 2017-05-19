using System;
using System.Globalization;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWeb.Cache.Policies {
	/// <summary>
	/// Isolate cache by language and timezone<br/>
	/// 按语言和时区隔离缓存<br/>
	/// </summary>
	/// <seealso cref="CacheFactory"/>
	/// <seealso cref="CacheFactoryOptions"/>
	public class CacheIsolateByLocale : ICacheIsolationPolicy {
		/// <summary>
		/// Get isolation key<br/>
		/// 获取隔离键<br/>
		/// </summary>
		/// <returns></returns>
		public object GetIsolationKey() {
			var language = CultureInfo.CurrentCulture.Name;
			var timezone = HttpManager.CurrentContext.GetData<TimeZoneInfo>(LocaleUtils.TimeZoneKey);
			return Pair.Create(language, timezone);
		}
	}
}
