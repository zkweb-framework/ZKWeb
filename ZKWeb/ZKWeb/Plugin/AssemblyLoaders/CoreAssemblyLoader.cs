#if NETCORE
using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace ZKWeb.Plugin.AssemblyLoaders {
	/// <summary>
	/// .Net Core使用的程序集载入器
	/// </summary>
	internal class CoreAssemblyLoader : IAssemblyLoader {
		/// <summary>
		/// 载入程序集使用的上下文
		/// </summary>
		private LoadContext Context { get; set; }
		/// <summary>
		/// 用于包装mscorlib的程序集名称的集合
		/// Roslyn的"IgnoreCorLibraryDuplicatedTypes"选项没有公开，所以需要在这里处理
		/// </summary>
		private HashSet<string> WrapperAssemblyNames { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		public CoreAssemblyLoader() {
			Context = new LoadContext();
			WrapperAssemblyNames = new HashSet<string>() {
				"System.Console"
			};
		}

		/// <summary>
		/// 获取当前已载入的程序集列表
		/// 排除仅用于包装的程序集和动态程序集
		/// </summary>
		public IList<Assembly> GetLoadedAssemblies() {
			return DependencyContext.Default.RuntimeLibraries
				.SelectMany(l => l.GetDefaultAssemblyNames(DependencyContext.Default))
				.Where(name => !WrapperAssemblyNames.Contains(name.Name))
				.Select(name => Context.LoadFromAssemblyName(name))
				.Where(assembly => !assembly.IsDynamic).ToList();
		}

		/// <summary>
		/// 根据名称载入程序集
		/// </summary>
		public Assembly Load(string name) {
			return Context.LoadFromAssemblyName(new AssemblyName(name));
		}

		/// <summary>
		/// 根据名称载入程序集
		/// </summary>
		public Assembly Load(AssemblyName assemblyName) {
			return Context.LoadFromAssemblyName(assemblyName);
		}

		/// <summary>
		/// 从二进制数据载入程序集
		/// </summary>
		public Assembly Load(byte[] rawAssembly) {
			using (var stream = new MemoryStream(rawAssembly)) {
				return Context.LoadFromStream(stream);
			}
		}

		/// <summary>
		/// 从文件载入程序集
		/// </summary>
		public Assembly LoadFile(string path) {
			return Context.LoadFromAssemblyPath(path);
		}

		/// <summary>
		/// 载入程序集使用的上下文
		/// </summary>
		private class LoadContext : AssemblyLoadContext {
			protected override Assembly Load(AssemblyName assemblyName) {
				try {
					// 尝试直接载入
					return Assembly.Load(assemblyName);
				} catch {
					// 失败时枚举插件的引用文件夹载入
					var pluginManager = Application.Ioc.Resolve<PluginManager>();
					foreach (var plugin in pluginManager.Plugins) {
						var path = plugin.ReferenceAssemblyPath(assemblyName.Name);
						if (path != null) {
							return LoadFromAssemblyPath(path);
						}
					}
					throw;
				}
			}
		}
	}
}
#endif
