using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKWeb.Core.Model {
	/// <summary>
	/// 插件的基本类
	/// 如何使用这个类
	/// class Plugin : PluginBase {
	///		public override void Load(PluginManager manager) {
	///			Register<ControllerBase>(new HelloController());
	///		}
	/// }
	/// </summary>
	public abstract class PluginBase {
		/// <summary>
		/// 事件处理器的索引
		/// </summary>
		protected ConcurrentDictionary<Type, List<IEventHandler>> handlers { get; set; }
		= new ConcurrentDictionary<Type, List<IEventHandler>>();

		/// <summary>
		/// 载入插件时的处理
		/// </summary>
		/// <param name="manager">网站程序</param>
		public virtual void Load(Application manager) { }

		/// <summary>
		/// 卸载插件时的处理
		/// </summary>
		/// <param name="manager">网站程序</param>
		public virtual void Unload(Application manager) { }

		/// <summary>
		/// 注册事件处理器
		/// </summary>
		/// <typeparam name="T">事件处理器的类型</typeparam>
		/// <param name="handler">处理器</param>
		public virtual void Register<T>(T handler)
			where T : IEventHandler {
			List<IEventHandler> handlersAddTo;
			if (!handlers.TryGetValue(typeof(T), out handlersAddTo)) {
				handlersAddTo = handlers[typeof(T)] = new List<IEventHandler>();
			}
			handlersAddTo.Add(handler);
		}

		/// <summary>
		/// 触发事件
		/// </summary>
		/// <typeparam name="T">事件处理器的类型</typeparam>
		/// <param name="args">参数</param>
		/// <param name="stop">是否停止之后的处理</param>
		public virtual void Trigger<T>(object args, ref bool stop)
			where T : IEventHandler {
			List<IEventHandler> handlersUse;
			if (!handlers.TryGetValue(typeof(T), out handlersUse)) {
				return;
			}
			foreach (var handler in handlersUse) {
				handler.Handle(args, ref stop);
				if (stop) {
					break;
				}
			}
		}
	}
}
