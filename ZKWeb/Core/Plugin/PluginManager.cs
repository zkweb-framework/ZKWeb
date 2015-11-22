using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace ZKWeb.Core.Plugin {
	/// <summary>
	/// 插件管理器
	/// 载入插件的流程
	///		从App_Code枚举文件夹，使用Csscript执行里面的Plugin.cs
	///		创建Plugin.cs的Plugin类的实例并添加到插件列表中
	/// </summary>
	public class PluginManager {
		/// <summary>
		/// 插件列表
		/// </summary>
		protected List<PluginBase> plugins { get; set; } = new List<PluginBase>();
		/// <summary>
		/// 插件列表，公开的属性
		/// </summary>
		public IEnumerable Plugins { get { return plugins; } }

		/// <summary>
		/// 载入所有插件并启动对插件文件的监控
		/// </summary>
		public PluginManager() {
			/* foreach (var dir in Directory.EnumerateDirectories("App_Code")) {
				var pluginEntryPath = Path.Combine(dir, "plugin.cs");
				if (File.Exists(pluginEntryPath)) {
					// ...
				}
			} */
			var reloader = new Thread(Reloader);
			reloader.IsBackground = true;
			reloader.Start();
		}

		/// <summary>
		/// 触发事件
		/// </summary>
		/// <typeparam name="T">事件处理器的类型</typeparam>
		/// <param name="args">参数</param>
		public virtual void Trigger<T>(object args)
			where T : IEventHandler {
			var plugins_copy = plugins;
			bool stop = false;
			foreach (var plugin in plugins_copy) {
				plugin.Trigger<T>(args, ref stop);
				if (stop) {
					break;
				}
			}
		}

		/// <summary>
		/// 在独立线程中监控插件文件，发生变化时重新加载
		/// </summary>
		protected virtual void Reloader() {
			var plugins_new = new List<PluginBase>();
			plugins = plugins_new;
		}
	}
}
