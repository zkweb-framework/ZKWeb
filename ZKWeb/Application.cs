using System;
using System.Threading;
using ZKWebStandard.Collections;
using ZKWebStandard.Ioc;

namespace ZKWeb {
	/// <summary>
	/// 主程序
	/// </summary>
	public class Application {
		/// <summary>
		/// 框架的完整版本
		/// </summary>
		public static string FullVersion { get { return "0.9.7 testing"; } }
		/// <summary>
		/// 框架的数值版本
		/// </summary>
		public static Version Version { get { return Version.Parse(FullVersion.Split(' ')[0]); } }
		/// <summary>
		/// 当前使用的容器
		/// </summary>
		public static IContainer Ioc { get { return overrideIoc.Value ?? defaultIoc; } }
		private static IContainer defaultIoc = new Container();
		private static ThreadLocal<IContainer> overrideIoc = new ThreadLocal<IContainer>();

		/// <summary>
		/// 重载当前使用的Ioc容器，在当前线程中有效
		/// 重载后的容器会继承原有的容器，但不会对原有的容器做出修改
		/// </summary>
		/// <returns></returns>
		public static IDisposable OverrideIoc() {
			var previousOverride = overrideIoc.Value;
			overrideIoc.Value = (IContainer)Ioc.Clone();
			return new SimpleDisposable(() => {
				var tmp = overrideIoc.Value;
				overrideIoc.Value = previousOverride;
				tmp.Dispose();
			});
		}
	}
}
