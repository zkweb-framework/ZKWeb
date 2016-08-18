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
		/// Assembly names that used for wrap mscorlib
		/// Because IgnoreCorLibraryDuplicatedTypes is a private option in Roslyn
		/// We need to use a black list
		/// </summary>
		private ISet<string> WrapperAssemblyNames { get; set; }
		/// <summary>
		/// Replacement assemblies
		/// </summary>
		private IDictionary<string, string> ReplacementAssemblies { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public CoreAssemblyLoader() {
			Context = new LoadContext();
			WrapperAssemblyNames = new HashSet<string>() {
				"System.Console",
				"System.Runtime.Extensions"
			};
			ReplacementAssemblies = new Dictionary<string, string>() {
				{ "System.FastReflection", "FastReflection" },
				{ "System.Drawing", "ZKWeb.System.Drawing" }
			};
		}

		/// <summary>
		/// Get loaded assemblies
		/// Except wrapper assemblies and dynamic assemblies
		/// </summary>
		public IList<Assembly> GetLoadedAssemblies() {
			return DependencyContext.Default.RuntimeLibraries
				.SelectMany(l => l.GetDefaultAssemblyNames(DependencyContext.Default))
				.Where(name => !WrapperAssemblyNames.Contains(name.Name))
				.Select(name => Context.LoadFromAssemblyName(name))
				.Where(assembly => !assembly.IsDynamic).ToList();
		}

		/// <summary>
		/// Load assembly by name
		/// </summary>
		public Assembly Load(string name) {
			// Replace name if replacement exists
			name = ReplacementAssemblies.GetOrDefault(name, name);
			return Context.LoadFromAssemblyName(new AssemblyName(name));
		}

		/// <summary>
		/// Load assembly by name object
		/// </summary>
		public Assembly Load(AssemblyName assemblyName) {
			return Context.LoadFromAssemblyName(assemblyName);
		}

		/// <summary>
		/// Load assembly from it's binary contents
		/// </summary>
		public Assembly Load(byte[] rawAssembly) {
			using (var stream = new MemoryStream(rawAssembly)) {
				return Context.LoadFromStream(stream);
			}
		}

		/// <summary>
		/// Load assembly from file path
		/// </summary>
		public Assembly LoadFile(string path) {
			return Context.LoadFromAssemblyPath(path);
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
