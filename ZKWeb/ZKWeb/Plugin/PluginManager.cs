using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ZKWeb.Plugin.AssemblyLoaders;
using ZKWeb.Server;
using ZKWeb.Storage;
using ZKWebStandard.Utils;

namespace ZKWeb.Plugin {
	/// <summary>
	/// Plugin manager<br/>
	/// 插件管理器<br/>
	/// </summary>
	public class PluginManager {
		/// <summary>
		/// Plugins<br/>
		/// 插件列表<br/>
		/// </summary>
		public virtual IList<PluginInfo> Plugins { get; protected set; }
		/// <summary>
		/// Plugin assemblies<br/>
		/// 插件程序集列表<br/>
		/// </summary>
		public virtual IList<Assembly> PluginAssemblies { get; protected set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public PluginManager() {
			Plugins = new List<PluginInfo>();
			PluginAssemblies = new List<Assembly>();
		}

		/// <summary>
		/// Load all plugins<br/>
		/// Flow<br/>
		/// - Get plugin names from website configuration<br/>
		/// - Load plugin information from it's directory<br/>
		/// - Use roslyn compile service compile the source files to assembly<br/>
		/// - Load compiled assembly<br/>
		/// - Register types in assembly to IoC container<br/>
		/// Attention<br/>
		/// - IPlugin will not initliaze here because we may need initialize database before<br/>
		///   you should invoke IPlugin manually after calling this method<br/>
		/// 加载所有插件<br/>
		/// 流程<br/>
		/// - 从网站配置获取插件名称列表<br/>
		/// - 从插件目录加载插件信息<br/>
		/// - 使用roslyn编译服务编译插件源代码到程序集<br/>
		/// - 加载编译后的程序集<br/>
		/// - 注册程序集中的类型到IoC容器<br/>
		/// 注意<br/>
		/// - 插件不会在这里初始化, 因为我们可能需要在这之前初始化数据库<br/>
		///   你需要在调用这个函数后手动调用IPlugin接口<br/>
		/// </summary>
		internal protected virtual void Initialize() {
			var configManager = Application.Ioc.Resolve<WebsiteConfigManager>();
			var pathManager = Application.Ioc.Resolve<LocalPathManager>();
			Plugins.Clear();
			PluginAssemblies.Clear();
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
				Plugins.Add(info);
			}
			// Load plugins
			var assemblyLoader = Application.Ioc.Resolve<IAssemblyLoader>();
			foreach (var plugin in Plugins) {
				// Compile plugin
				plugin.Compile();
				// Load compiled assembly, some plugin may not have an assembly
				var assemblyPath = plugin.AssemblyPath();
				if (File.Exists(assemblyPath)) {
					var assembly = assemblyLoader.LoadFile(assemblyPath);
					plugin.Assembly = assembly;
					PluginAssemblies.Add(assembly);
				}
			}
			// Register types in assembly to IoC container
			// Only public types will be registered
			foreach (var assembly in PluginAssemblies) {
				var types = assembly.GetTypes().Where(t => t.IsPublic);
				Application.Ioc.RegisterExports(types);
			}
		}
	}
}
