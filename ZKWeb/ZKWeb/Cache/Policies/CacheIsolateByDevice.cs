using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

namespace ZKWeb.Cache.Policies {
	/// <summary>
	/// Isolate cache by client device type, eg Desktop or Mobile
	/// </summary>
	public class CacheIsolateByDevice : ICacheIsolationPolicy {
		/// <summary>
		/// Get isolation key
		/// </summary>
		/// <returns></returns>
		public object GetIsolationKey() {
			return HttpManager.CurrentContext.GetClientDevice();
		}
	}
}
