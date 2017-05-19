using System.Linq;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWeb.Cache.Policies {
	/// <summary>
	/// Isolate cache by http request url and parameters<br/>
	/// 根据Http请求的Url和参数隔离缓存<br/>
	/// </summary>
	/// <seealso cref="CacheFactory"/>
	/// <seealso cref="CacheFactoryOptions"/>
	public class CacheIsolateByUrl : ICacheIsolationPolicy {
		/// <summary>
		/// Get isolation key<br/>
		/// 获取隔离键<br/>
		/// </summary>
		/// <returns></returns>
		public object GetIsolationKey() {
			var context = HttpManager.CurrentContext;
			var request = context.Request;
			var key = request.Path;
			if (request.GetFormValues().Any()) {
				// use form values + query string
				key += '?';
				key += HttpUtils.BuildQueryString(request.GetAllDictionary());
			} else if (!string.IsNullOrEmpty(request.QueryString)) {
				// use query string
				key += request.QueryString;
			}
			return key;
		}
	}
}
