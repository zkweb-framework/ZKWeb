using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace ZKWeb.Core {
	/// <summary>
	/// 用于重新载入插件和网站配置的线程
	/// </summary>
	public static class Reloader {
		/// <summary>
		/// 启动用于重新载入插件和网站配置的线程
		/// </summary>
		/// <param name="application"></param>
		public static void Start(Application application) {
			var thread = new Thread(() => { });
			thread.IsBackground = true;
			thread.Start();
		}
	}
}
