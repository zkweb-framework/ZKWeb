using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

namespace ZKWeb.Cache.Policies {
	/// <summary>
	/// Isolate cache by client device type, eg Desktop or Mobile<br/>
	/// 按客户端的设备类型隔离缓存, 例如Desktop或者Mobile<br/>
	/// </summary>
	/// <seealso cref="CacheFactory"/>
	/// <seealso cref="CacheFactoryOptions"/>
	public class CacheIsolateByDevice : ICacheIsolationPolicy {
		/// <summary>
		/// Get isolation key<br/>
		/// 获取隔离键<br/>
		/// </summary>
		/// <returns></returns>
		public object GetIsolationKey() {
			return HttpManager.CurrentContext.GetClientDevice();
		}
	}
}
