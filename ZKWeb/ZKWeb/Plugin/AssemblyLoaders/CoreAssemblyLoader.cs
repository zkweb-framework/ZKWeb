#if NETCORE
using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using ZKWebStandard.Extensions;

namespace ZKWeb.Plugin.AssemblyLoaders {
	/// <summary>
	/// Assembly loader for .Net Core
	/// </summary>
	internal class CoreAssemblyLoader : IAssemblyLoader {
		/// <summary>
		/// The load context
		/// </summary>
		private LoadContext Context { get; set; }
		/// <summary>
		/// Replacement assemblies
		/// </summary>
		private IDictionary<string, string> ReplacementAssemblies { get; set; }
		/// <summary>
		/// Loaded assemblies
		/// .Net Core is not able to track loaded assemblies, so we will track them manually
		/// </summary>
		private ISet<Assembly> LoadedAssemblies { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public CoreAssemblyLoader() {
			Context = new LoadContext();
			ReplacementAssemblies = new Dictionary<string, string>() {
				{ "System.FastReflection", "FastReflection" },
				{ "System.DrawingCore", "ZKWeb.System.Drawing" }
			};
			LoadedAssemblies = new HashSet<Assembly>(
				DependencyContext.Default.RuntimeLibraries
				.SelectMany(l => l.GetDefaultAssemblyNames(DependencyContext.Default))
				.Select(name => Context.LoadFromAssemblyName(name))
				.Where(assembly => !assembly.IsDynamic));
			// for `dynamic` support
			Load("Microsoft.CSharp");
		}

		/// <summary>
		/// Get loaded assemblies
		/// Except wrapper assemblies and dynamic assemblies
		/// </summary>
		public IList<Assembly> GetLoadedAssemblies() {
			return LoadedAssemblies.ToList();
		}

		/// <summary>
		/// Load assembly by name
		/// </summary>
		public Assembly Load(string name) {
			// Replace name if replacement exists
			name = ReplacementAssemblies.GetOrDefault(name, name);
			var assembly = Context.LoadFromAssemblyName(new AssemblyName(name));
			LoadedAssemblies.Add(assembly);
			return assembly;
		}

		/// <summary>
		/// Load assembly by name object
		/// </summary>
		public Assembly Load(AssemblyName assemblyName) {
			var assembly = Context.LoadFromAssemblyName(assemblyName);
			LoadedAssemblies.Add(assembly);
			return assembly;
		}

		/// <summary>
		/// Load assembly from it's binary contents
		/// </summary>
		public Assembly Load(byte[] rawAssembly) {
			using (var stream = new MemoryStream(rawAssembly)) {
				var assembly = Context.LoadFromStream(stream);
				LoadedAssemblies.Add(assembly);
				return assembly;
			}
		}

		/// <summary>
		/// Load assembly from file path
		/// </summary>
		public Assembly LoadFile(string path) {
			var assembly = Context.LoadFromAssemblyPath(path);
			LoadedAssemblies.Add(assembly);
			return assembly;
		}

		/// <summary>
		/// The context for loading assembly
		/// </summary>
		private class LoadContext : AssemblyLoadContext {
			protected override Assembly Load(AssemblyName assemblyName) {
				try {
					// Try load directly
					return Assembly.Load(assemblyName);
				} catch {
					// If failed, try to load it from plugin's reference directory
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
