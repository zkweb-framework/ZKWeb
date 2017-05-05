#if NETCORE
using Microsoft.Extensions.DependencyModel;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using ZKWebStandard.Extensions;

namespace ZKWeb.Plugin.AssemblyLoaders {
	/// <summary>
	/// Assembly loader for .Net Core
	/// </summary>
	internal class CoreAssemblyLoader : AssemblyLoaderBase {
		/// <summary>
		/// The load context
		/// </summary>
		private LoadContext Context { get; set; }

		/// <summary>
		/// Initialize
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
		/// Load assembly by name
		/// </summary>
		public override Assembly Load(string name) {
			name = ReplacementAssemblies.GetOrDefault(name, name);
			var assembly = Context.LoadFromAssemblyName(new AssemblyName(name));
			return HandleLoadedAssembly(assembly);
		}

		/// <summary>
		/// Load assembly by name object
		/// </summary>
		public override Assembly Load(AssemblyName assemblyName) {
			var assembly = Context.LoadFromAssemblyName(assemblyName);
			return HandleLoadedAssembly(assembly);
		}

		/// <summary>
		/// Load assembly from it's binary contents
		/// </summary>
		public override Assembly Load(byte[] rawAssembly) {
			using (var stream = new MemoryStream(rawAssembly)) {
				var assembly = Context.LoadFromStream(stream);
				return HandleLoadedAssembly(assembly);
			}
		}

		/// <summary>
		/// Load assembly from file path
		/// </summary>
		public override Assembly LoadFile(string path) {
			var assembly = Context.LoadFromAssemblyPath(path);
			return HandleLoadedAssembly(assembly);
		}

		/// <summary>
		/// Assembly loading context
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
