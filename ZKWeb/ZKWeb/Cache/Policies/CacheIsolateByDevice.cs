using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

namespace ZKWeb.Cache.Policies {
	/// <summary>
	/// 按当前设备隔离缓存
	/// </summary>
	public class CacheIsolateByDevice : ICacheIsolationPolicy {
		/// <summary>
		/// 获取隔离键
		/// </summary>
		/// <returns></returns>
		public object GetIsolationKey() {
			return HttpManager.CurrentContext.GetClientDevice();
		}
	}
}
