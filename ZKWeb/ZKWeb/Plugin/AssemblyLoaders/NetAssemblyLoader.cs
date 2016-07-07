#if !NETCORE
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ZKWeb.Plugin.AssemblyLoaders {
	/// <summary>
	/// .Net Framework使用的程序集载入器
	/// </summary>
	internal class NetAssemblyLoader : IAssemblyLoader {
		/// <summary>
		/// 初始化
		/// </summary>
		public NetAssemblyLoader() {
			// 注册解决程序集依赖的函数
			AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolver;
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;
		}

		/// <summary>
		/// 获取当前已载入的程序集列表
		/// </summary>
		public IList<Assembly> GetLoadedAssemblies() {
			return AppDomain.CurrentDomain.GetAssemblies();
		}

		/// <summary>
		/// 根据名称载入程序集
		/// </summary>
		public Assembly Load(string name) {
			return Assembly.Load(name);
		}

		/// <summary>
		/// 从原始数据载入程序集
		/// </summary>
		public Assembly Load(byte[] rawAssembly) {
			return Assembly.Load(rawAssembly);
		}

		/// <summary>
		/// 从文件载入程序集
		/// </summary>
		public Assembly LoadFile(string path) {
			return Assembly.LoadFile(path);
		}

		/// <summary>
		/// 程序集解决器
		/// 从插件目录下搜索程序集并载入
		/// </summary>
		/// <returns></returns>
		protected Assembly AssemblyResolver(object sender, ResolveEventArgs args) {
			// 从已载入的程序集中查找
			var requireName = new AssemblyName(args.Name);
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				if (assembly.GetName().Name == requireName.Name) {
					return assembly;
				}
			}
			// 查找插件的引用程序集目录
			// 这里不查找插件的程序集目录避免错误的在重新编译前载入了插件的程序集
			var pluginManager = Application.Ioc.Resolve<PluginManager>();
			foreach (var plugin in pluginManager.Plugins) {
				if (plugin.References.Contains(requireName.Name)) {
					var path = plugin.ReferenceAssemblyPath(requireName.Name);
					return Assembly.LoadFrom(path);
				}
			}
			// 找不到时返回null
			return null;
		}
	}
}
#endif
