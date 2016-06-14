using ZKWebStandard.Web;

namespace ZKWeb.Cache.Policies {
	/// <summary>
	/// 按当前Url和请求参数隔离缓存
	/// </summary>
	public class CacheIsolateByUrl : ICacheIsolationPolicy {
		/// <summary>
		/// 获取隔离键
		/// </summary>
		/// <returns></returns>
		public object GetIsolationKey() {
			var request = HttpManager.CurrentContext.Request;
			return request.Path + request.QueryString;
		}
	}
}
