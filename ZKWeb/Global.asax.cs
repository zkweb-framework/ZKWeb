using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using ZKWeb.Core;
using ZKWeb.Core.Model;

namespace ZKWeb {
	/// <summary>
	/// 网站的入口点
	/// TODO:
	/// log monitor
	/// database migrate
	/// plugin break point test
	/// template engine
	/// session, cookies, csrf manager
	/// 
	/// namespace problem (keep App_Code?)
	/// plugin name (as folder name)
	/// plugin priority (use static list or?)
	/// 
	/// </summary>
	public class Application : HttpApplication {
		/// <summary>
		/// 插件管理器
		/// </summary>
		public PluginManager PluginManager { get; protected set; }

		/// <summary>
		/// 网站启动时的处理
		/// 初始化插件管理器
		/// </summary>
		public void Application_Start() {
			PluginManager = new PluginManager();
		}

		/// <summary>
		/// 收到Http请求时的处理
		/// 让插件处理器处理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_BeginRequest(object sender, EventArgs e) {
			PluginManager.TriggerReversed<ControllerBase>(this);
		}

		/// <summary>
		/// 捕获到例外时的处理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Application_Error(object sender, EventArgs e) {
			Response.Write(Server.GetLastError()?.ToString());
		}
	}
}
