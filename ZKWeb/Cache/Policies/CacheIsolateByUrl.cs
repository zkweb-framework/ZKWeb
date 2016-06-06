using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Cache.Interfaces;
using ZKWeb.Utils.Functions;

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
			return HttpContextUtils.CurrentContext.Request.Url.PathAndQuery;
		}
	}
}
