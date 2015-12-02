using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using ZKWeb.Core.Model;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Core {
	/// <summary>
	/// 插件管理器
	/// 载入插件的流程
	///		枚举配置文件中的Plugins
	///		使用Csscript执行插件目录下的Plugin.cs并创建Plugin类的实例
	///		把创建的实例添加到插件列表中
	/// </summary>
	public class PluginManager {
		/// <summary>
		/// 插件列表
		/// </summary>
		public IEnumerable<PluginBase> Plugins { get; protected set; } =
			new List<PluginBase>();
		/// <summary>
		/// 插件列表的线程锁
		/// </summary>
		public ReaderWriterLockSlim PluginsLock { get; protected set; } =
			new ReaderWriterLockSlim();

		/// <summary>
		/// 载入所有插件并启动对插件文件的监控
		/// </summary>
		public PluginManager() {
			Reload();
			var reloader = new Thread(Reloader);
			reloader.IsBackground = true;
			reloader.Start();
		}

		/// <summary>
		/// 触发事件，按插件的定义顺序出发
		/// 一般用于触发依赖性的事件，例如添加网页元素
		/// </summary>
		/// <typeparam name="T">事件处理器的类型</typeparam>
		/// <param name="args">参数</param>
		public virtual void Trigger<T>(object args)
			where T : IEventHandler {
			PluginsLock.EnterReadLock();
			try {
				bool stop = false;
				foreach (var plugin in Plugins) {
					plugin.Trigger<T>(args, ref stop);
					if (stop) {
						break;
					}
				}
			} finally {
				PluginsLock.ExitReadLock();
			}
		}

		/// <summary>
		/// 触发事件，按插件的定义顺序的反序触发
		/// 一般用于触发覆盖性的事件，例如Http请求
		/// </summary>
		/// <typeparam name="T">事件处理器的类型</typeparam>
		/// <param name="args"></param>
		public virtual void TriggerReversed<T>(object args)
			where T : IEventHandler {
			PluginsLock.EnterReadLock();
			try {
				bool stop = false;
				foreach (var plugin in Enumerable.Reverse(Plugins)) {
					plugin.Trigger<T>(args, ref stop);
					if (stop) {
						break;
					}
				}
			} finally {
				PluginsLock.ExitReadLock();
			}
		}

		/// <summary>
		/// 重新加载所有插件
		/// </summary>
		protected virtual void Reload() {
			PluginsLock.EnterWriteLock();
			try {
				throw new NotImplementedException();
			} finally {
				PluginsLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// 在独立线程中监控插件文件，发生变化时重新加载
		/// </summary>
		protected virtual void Reloader() {
			throw new NotImplementedException();
		}
	}
}
