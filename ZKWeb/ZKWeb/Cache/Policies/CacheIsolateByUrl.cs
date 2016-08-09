using System.Linq;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWeb.Cache.Policies {
	/// <summary>
	/// Isolate cache by visiting url and query string
	/// </summary>
	public class CacheIsolateByUrl : ICacheIsolationPolicy {
		/// <summary>
		/// Get isolation key
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
