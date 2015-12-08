using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace ZKWeb.Manager {
	/// <summary>
	/// 用于重新载入插件和网站配置的线程
	/// </summary>
	public static class Reloader {
		/// <summary>
		/// 检测以下文件是否有改变，有改变时卸载当前程序域来让下次打开网站时重新载入
		///		插件源代码文件 Plugins\*\src\*.cs
		///		插件配置文件 Plugins\*\plugin.json
		///		网站配置文件 App_Data\config.json
		/// </summary>
		/// <param name="application"></param>
		public static void Start(Application application) {
			var thread = new Thread(() => {
				// 待编写
			});
			thread.IsBackground = true;
			thread.Start();
		}
	}
}
