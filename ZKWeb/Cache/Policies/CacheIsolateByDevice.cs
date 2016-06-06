using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Cache.Policies {
	/// <summary>
	/// 按当前设备隔离缓存
	/// </summary>
	public class CacheIsolateByDevice {
		/// <summary>
		/// 获取隔离键
		/// </summary>
		/// <returns></returns>
		public object GetIsolationKey() {
			return HttpDeviceUtils.GetClientDevice();
		}
	}
}
