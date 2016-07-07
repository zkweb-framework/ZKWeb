using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ZKWeb.Plugin.AssemblyLoaders;
using ZKWeb.Server;
using ZKWebStandard.Utils;

namespace ZKWeb.Plugin {
	/// <summary>
	/// 插件管理器
	/// </summary>
	public class PluginManager {
		/// <summary>
		/// 插件列表
		/// </summary>
		public virtual IList<PluginInfo> Plugins { get; protected set; }
		/// <summary>
		/// 插件程序集列表
		/// </summary>
		public virtual IList<Assembly> PluginAssemblies { get; protected set; }

		/// <summary>
		/// 初始化
		/// </summary>
		public PluginManager() {
			Plugins = new List<PluginInfo>();
			PluginAssemblies = new List<Assembly>();
		}

		/// <summary>
		/// 载入所有插件
		/// 载入插件的流程
		///		枚举配置文件中的Plugins
		///		载入Plugins.json中的插件信息
		///		使用Csscript编译插件目录下的源代码到dll
		///		载入编译好的dll
		///		注册dll中的类型到Ioc中
		/// 注意
		///		载入插件后因为需要继续初始化数据库等，所以不会立刻执行IPlugin中的处理
		///		IPlugin中的处理需要在创建之后手动执行
		/// </summary>
		internal static void Initialize() {
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			var pathManager = Application.Ioc.Resolve<PathManager>();
			var pluginManager = Application.Ioc.Resolve<PluginManager>();
			pluginManager.Plugins.Clear();
			pluginManager.PluginAssemblies.Clear();
			// 获取网站配置中的插件列表并载入插件信息
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
			// 载入插件
			var assemblyLoader = Application.Ioc.Resolve<IAssemblyLoader>();
			foreach (var plugin in pluginManager.Plugins) {
				// 编译带源代码的插件
				plugin.Compile();
				// 载入插件程序集，注意部分插件只有资源文件没有程序集
				var assemblyPath = plugin.AssemblyPath();
				if (File.Exists(assemblyPath)) {
					pluginManager.PluginAssemblies.Add(assemblyLoader.LoadFile(assemblyPath));
				}
			}
			// 注册程序集中的类型到容器中
			// 只有公开的类型会被注册
			foreach (var assembly in pluginManager.PluginAssemblies) {
				var types = assembly.GetTypes().Where(t => t.GetTypeInfo().IsPublic);
				Application.Ioc.RegisterExports(types);
			}
		}
	}
}
