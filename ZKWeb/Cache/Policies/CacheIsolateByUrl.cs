using System.Linq;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
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
			// 从路径和请求参数生成
			var context = HttpManager.CurrentContext;
			var request = context.Request;
			var key = request.Path;
			if (request.GetFormValues().Any()) {
				// 有表单值时，不能只以Url参数隔离，需要以全部参数隔离
				key += '?';
				key += HttpUtils.BuildQueryString(request.GetAllDictionary());
			} else if (!string.IsNullOrEmpty(request.QueryString)) {
				// 没有表单值，但是有Url参数时，需要以Url参数隔离
				key += request.QueryString;
			}
			return key;
		}
	}
}
