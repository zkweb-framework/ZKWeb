#if !NETCORE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZKWeb.Plugin.AssemblyLoaders {
	/// <summary>
	/// Assembly loader for .Net Framework
	/// </summary>
	internal class NetAssemblyLoader : IAssemblyLoader {
		/// <summary>
		/// Possible assembly name suffixes
		/// Use to load assemblies by short name
		/// </summary>
		protected IList<string> PossibleAssemblyNameSuffixes { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public NetAssemblyLoader() {
			// Register assembly resolve event
			AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolver;
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;
			// Add possible assembly name suffixes
			PossibleAssemblyNameSuffixes = new List<string>() {
				", Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
				", Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
			};
		}

		/// <summary>
		/// Get loaded assemblies
		/// Except dynamic assemblies
		/// </summary>
		public IList<Assembly> GetLoadedAssemblies() {
			return AppDomain.CurrentDomain.GetAssemblies()
				.Where(assembly => !assembly.IsDynamic).ToList();
		}

		/// <summary>
		/// Load assembly by name
		/// </summary>
		public Assembly Load(string name) {
			try {
				// Try load directly
				return Load(new AssemblyName(name));
			} catch {
				// If load failed, add suffixes and try again
				foreach (var suffix in PossibleAssemblyNameSuffixes) {
					try {
						return Load(new AssemblyName(name + suffix));
					} catch {
					}
				}
				throw;
			}
		}

		/// <summary>
		/// Load assembly by name object
		/// </summary>
		public Assembly Load(AssemblyName assemblyName) {
			return Assembly.Load(assemblyName);
		}

		/// <summary>
		/// Load assembly from it's binary contents
		/// </summary>
		public Assembly Load(byte[] rawAssembly) {
			return Assembly.Load(rawAssembly);
		}

		/// <summary>
		/// Load assembly from file path
		/// </summary>
		public Assembly LoadFile(string path) {
			return Assembly.LoadFile(path);
		}

		/// <summary>
		/// Assembly resolve event handler
		/// </summary>
		/// <returns></returns>
		protected Assembly AssemblyResolver(object sender, ResolveEventArgs args) {
			// If assembly already loaded, return the loaded instance
			var requireName = new AssemblyName(args.Name);
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				if (assembly.GetName().Name == requireName.Name) {
					return assembly;
				}
			}
			// Try to load Assembly from plugin's reference directory
			var pluginManager = Application.Ioc.Resolve<PluginManager>();
			foreach (var plugin in pluginManager.Plugins) {
				var path = plugin.ReferenceAssemblyPath(requireName.Name);
				if (path != null) {
					return Assembly.LoadFrom(path);
				}
			}
			// It's not found, return null
			return null;
		}
	}
}
#endif
