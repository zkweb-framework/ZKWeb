using CSScriptLibrary;
using DryIoc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using ZKWeb.Core.Model;
using ZKWeb.Model;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Manager {
	/// <summary>
	/// 插件管理器
	/// 载入插件的流程
	///		枚举配置文件中的Plugins
	///		载入Plugins.json中的插件信息
	///		使用Csscript编译插件目录下的源代码到dll
	///		载入编译好的dll并查找Plugin类名
	///		创建这个类型的对象以初始化插件
	/// </summary>
	public class PluginManager {
		/// <summary>
		/// 插件列表
		/// </summary>
		public List<PluginInfo> Plugins { get; private set; } = new List<PluginInfo>();

		/// <summary>
		/// 载入所有插件
		/// </summary>
		public PluginManager() {
			// 获取网站配置中的插件列表
			var configManager = Application.Current.Ioc.Resolve<ConfigManager>();
			// 载入所有插件信息
			foreach (var pluginName in configManager.WebsiteConfig.Plugins) {
				var dir = PathUtils.SecureCombine(PathConfig.PluginsRootDirectory, pluginName);
				var info = PluginInfo.FromDirectory(dir);
				Plugins.Add(info);
			}
			// 注册解决程序集依赖的函数
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;
			// 编译插件的程序集，载入并初始化插件
			foreach (var plugin in Plugins) {
				if (plugin.RequireRecompile()) {
					CSScript.CompileFiles(plugin.SourceFiles(), plugin.AssemblyPath(), false, plugin.RefAssemblies());
				}

			}
		}

		/// <summary>
		/// 程序集解决器
		/// 从插件目录下搜索程序集并载入
		/// </summary>
		/// <returns></returns>
		public Assembly AssemblyResolver(object sender, ResolveEventArgs args) {
			// 从已载入的程序集中查找
			var requireName = new AssemblyName(args.Name);
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				if (assembly.GetName().Name == requireName.Name) {
					return assembly;
				}
			}
			// 从各个插件文件夹中查找
			foreach (var plugin in Plugins) {
				/* var path = Path.Combine(directory.FullPath, $"{requireName.Name}.dll");
				if (File.Exists(path)) {
					return Assembly.LoadFrom(path);
				} */
			}
			// 找不到时返回null
			return null;
		}
	}
}
