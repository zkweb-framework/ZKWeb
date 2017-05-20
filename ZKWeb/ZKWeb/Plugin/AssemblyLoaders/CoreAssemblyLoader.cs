#if NETCORE
using Microsoft.Extensions.DependencyModel;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using ZKWebStandard.Extensions;

namespace ZKWeb.Plugin.AssemblyLoaders {
	/// <summary>
	/// Assembly loader for .Net Core<br/>
	/// .Net Core使用的程序集加载器<br/>
	/// </summary>
	internal class CoreAssemblyLoader : AssemblyLoaderBase {
		/// <summary>
		/// The load context<br/>
		/// 加载上下文<br/>
		/// </summary>
		private LoadContext Context { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public CoreAssemblyLoader() {
			Context = new LoadContext();
			foreach (var library in DependencyContext.Default.RuntimeLibraries) {
				foreach (var assemblyName in library.GetDefaultAssemblyNames(DependencyContext.Default)) {
					Load(assemblyName);
				}
			}
		}

		/// <summary>
		/// Load assembly by name<br/>
		/// 根据名称加载程序集<br/>	
		/// </summary>
		public override Assembly Load(string name) {
			name = ReplacementAssemblies.GetOrDefault(name, name);
			var assembly = Context.LoadFromAssemblyName(new AssemblyName(name));
			return HandleLoadedAssembly(assembly);
		}

		/// <summary>
		/// Load assembly by name object<br/>
		/// 根据名称对象加载程序集<br/>
		/// </summary>
		public override Assembly Load(AssemblyName assemblyName) {
			var assembly = Context.LoadFromAssemblyName(assemblyName);
			return HandleLoadedAssembly(assembly);
		}

		/// <summary>
		/// Load assembly from it's binary contents<br/>
		/// 根据二进制内容加载程序集<br/>
		/// </summary>
		public override Assembly Load(byte[] rawAssembly) {
			using (var stream = new MemoryStream(rawAssembly)) {
				var assembly = Context.LoadFromStream(stream);
				return HandleLoadedAssembly(assembly);
			}
		}

		/// <summary>
		/// Load assembly from file path<br/>
		/// 根据文件路径加载程序集<br/>
		/// </summary>
		public override Assembly LoadFile(string path) {
			var assembly = Context.LoadFromAssemblyPath(path);
			return HandleLoadedAssembly(assembly);
		}

		/// <summary>
		/// Assembly loading context<br/>
		/// 程序集的加载上下文<br/>
		/// </summary>
		private class LoadContext : AssemblyLoadContext {
			protected override Assembly Load(AssemblyName assemblyName) {
				try {
					// load it directly
					return Assembly.Load(assemblyName);
				} catch {
					// if failed, try to load it from reference directory under plugin directory
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
