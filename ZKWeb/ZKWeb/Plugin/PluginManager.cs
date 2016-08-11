using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ZKWeb.Plugin.AssemblyLoaders;
using ZKWeb.Server;
using ZKWebStandard.Utils;

namespace ZKWeb.Plugin {
	/// <summary>
	/// Plugin manager
	/// </summary>
	public class PluginManager {
		/// <summary>
		/// Plugins
		/// </summary>
		public virtual IList<PluginInfo> Plugins { get; protected set; }
		/// <summary>
		/// Plugin assemblies
		/// </summary>
		public virtual IList<Assembly> PluginAssemblies { get; protected set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public PluginManager() {
			Plugins = new List<PluginInfo>();
			PluginAssemblies = new List<Assembly>();
		}

		/// <summary>
		/// Load all plugins
		/// Flow
		/// - Get plugin names from website configuration
		/// - Load plugin information from it's directory
		/// - Use roslyn compile service compile the source files to assembly
		/// - Load compiled assembly
		/// - Register types in assembly to IoC container
		/// Attention
		/// - IPlugin will not initliaze here because we may need initialize database before
		///   you should invoke IPlugin manually after calling this method
		/// </summary>
		internal static void Initialize() {
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			var pathManager = Application.Ioc.Resolve<PathManager>();
			var pluginManager = Application.Ioc.Resolve<PluginManager>();
			pluginManager.Plugins.Clear();
			pluginManager.PluginAssemblies.Clear();
			// Get plugin names from website configuration
			var pluginDirectories = pathManager.GetPluginDirectories();
			foreach (var pluginName in configManager.WebsiteConfig.Plugins) {
				var dir = pluginDirectories
					.Select(p => PathUtils.SecureCombine(p, pluginName))
					.FirstOrDefault(p => Directory.Exists(p));
				if (dir == null) {
					throw new DirectoryNotFoundException($"Plugin directory of {pluginName} not found");
				}
				var info = PluginInfo.FromDirectory(dir);
				pluginManager.Plugins.Add(info);
			}
			// Load plugins
			var assemblyLoader = Application.Ioc.Resolve<IAssemblyLoader>();
			foreach (var plugin in pluginManager.Plugins) {
				// Compile plugin
				plugin.Compile();
				// Load compiled assembly, some plugin may not have an assembly
				var assemblyPath = plugin.AssemblyPath();
				if (File.Exists(assemblyPath)) {
					pluginManager.PluginAssemblies.Add(assemblyLoader.LoadFile(assemblyPath));
				}
			}
			// Register types in assembly to IoC container
			// Only public types will be registered
			foreach (var assembly in pluginManager.PluginAssemblies) {
				var types = assembly.GetTypes().Where(t => t.GetTypeInfo().IsPublic);
				Application.Ioc.RegisterExports(types);
			}
		}
	}
}
