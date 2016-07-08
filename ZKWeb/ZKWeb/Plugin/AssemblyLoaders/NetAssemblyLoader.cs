#if !NETCORE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZKWeb.Plugin.AssemblyLoaders {
	/// <summary>
	/// .Net Framework使用的程序集载入器
	/// </summary>
	internal class NetAssemblyLoader : IAssemblyLoader {
		/// <summary>
		/// 可能的程序集名称的后缀
		/// 用于载入缩写的程序集名称
		/// </summary>
		protected IList<string> PossibleAssemblyNameSuffix { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		public NetAssemblyLoader() {
			// 注册解决程序集依赖的函数
			AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolver;
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;
			// 可能的程序集名称的后缀
			PossibleAssemblyNameSuffix = new List<string>() {
				", Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
				", Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
			};
		}

		/// <summary>
		/// 获取当前已载入的程序集列表
		/// 排除动态程序集
		/// </summary>
		public IList<Assembly> GetLoadedAssemblies() {
			return AppDomain.CurrentDomain.GetAssemblies()
				.Where(assembly => !assembly.IsDynamic).ToList();
		}

		/// <summary>
		/// 根据名称载入程序集
		/// </summary>
		public Assembly Load(string name) {
			try {
				// 尝试直接载入
				return Load(new AssemblyName(name));
			} catch {
				// 直接载入失败时，添加可能的后缀再尝试载入
				foreach (var suffix in PossibleAssemblyNameSuffix) {
					try {
						return Load(new AssemblyName(name + suffix));
					} catch {
					}
				}
				throw;
			}
		}

		/// <summary>
		/// 根据名称载入程序集
		/// </summary>
		public Assembly Load(AssemblyName assemblyName) {
			return Assembly.Load(assemblyName);
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
				var path = plugin.ReferenceAssemblyPath(requireName.Name);
				if (path != null) {
					return Assembly.LoadFrom(path);
				}
			}
			// 找不到时返回null
			return null;
		}
	}
}
#endif
