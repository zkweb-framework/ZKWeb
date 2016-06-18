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
		/// 缓存隔离键时使用的键名
		/// 缓存在上下文时会忽略Fetch标签指定的Url并使用原始Url缓存
		/// </summary>
		public string UrlKey = "ZKWeb.CacheIsolateByUrlKey";

		/// <summary>
		/// 获取隔离键
		/// </summary>
		/// <returns></returns>
		public object GetIsolationKey() {
			// 从缓存获取
			var context = HttpManager.CurrentContext;
			var key = context.GetData<string>(UrlKey);
			if (key != null) {
				return key;
			}
			// 从路径和请求参数生成
			var request = context.Request;
			key = request.Path;
			if (request.GetFormValues().Any()) {
				// 有表单值时，不能只以Url参数隔离，需要以全部参数隔离
				key += '?';
				key += HttpUtils.BuildQueryString(request.GetAllDictionary());
			} else if (!string.IsNullOrEmpty(request.QueryString)) {
				// 没有表单值，但是有Url参数时，需要以Url参数隔离
				key += request.QueryString;
			}
			// 保存到缓存
			context.PutData(UrlKey, key);
			return key;
		}
	}
}
