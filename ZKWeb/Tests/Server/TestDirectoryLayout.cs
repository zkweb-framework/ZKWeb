using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Plugin;
using ZKWeb.Server;
using ZKWeb.Utils.Functions;
using ZKWeb.Utils.IocContainer;

namespace ZKWeb.Tests.Server {
	class TestDirectoryLayout : IDisposable {
		private IDisposable OverrideIoc { get; set; }
		private WebsiteConfig WebsiteConfig { get; set; }
		public IList<string> CleanupPaths { get; private set; }

		/// <summary>
		/// 测试时重载插件文件夹的设置
		/// 测试完毕后会删除所有插件目录
		/// </summary>
		/// <param name="pluginDirectories">插件目录列表，不传入时使用[ "AppData/__TestPlugins" ]</param>
		/// <param name="plugins">插件列表，不传入时使用[ "PluginA", "PluginB" ]</param>
		/// <param name="extra">附加数据，不传入时使用空的附加数据</param>
		/// <param name="pluginDirectoryIndex">创建插件文件夹时，使用的插件目录的序号</param>
		public TestDirectoryLayout(
			IList<string> pluginDirectories = null,
			IList<string> plugins = null,
			IDictionary<string, object> extra = null,
			int pluginDirectoryIndex = 0) {
			OverrideIoc = Application.OverrideIoc();
			WebsiteConfig = new WebsiteConfig() {
				PluginDirectories = pluginDirectories ?? new List<string>() { "AppData/__TestPlugins" },
				Plugins = plugins ?? new List<string>() { "PluginA", "PluginB" },
				Extra = extra ?? new Dictionary<string, object>()
			};
			CleanupPaths = new List<string>();
			// 重新注册ConfigManager, PathConfig, PathManager, PluginManager
			var configManagerMock = new Mock<ConfigManager>();
			configManagerMock.Setup(c => c.WebsiteConfig).Returns(WebsiteConfig);
			Application.Ioc.Unregister<ConfigManager>();
			Application.Ioc.Unregister<PathConfig>();
			Application.Ioc.Unregister<PathManager>();
			Application.Ioc.Unregister<PluginManager>();
			Application.Ioc.RegisterInstance(configManagerMock.Object);
			Application.Ioc.RegisterMany<PathConfig>(ReuseType.Singleton);
			Application.Ioc.RegisterMany<PathManager>(ReuseType.Singleton);
			Application.Ioc.RegisterMany<PluginManager>(ReuseType.Singleton);
			// 创建插件文件夹，并设置释放时删除
			// 创建插件文件夹后把该插件的信息加到插件管理器
			var pathManager = Application.Ioc.Resolve<PathManager>();
			var pluginManager = Application.Ioc.Resolve<PluginManager>();
			var pluginDirectory = pathManager.GetPluginDirectories()[pluginDirectoryIndex];
			foreach (var plugin in WebsiteConfig.Plugins) {
				var directory = Path.Combine(pluginDirectory, plugin);
				Directory.CreateDirectory(directory);
				pluginManager.Plugins.Add(PluginInfo.FromDirectory(directory));
			}
			foreach (var directory in pathManager.GetPluginDirectories()) {
				CleanupPaths.Add(directory);
			}
		}

		/// <summary>
		/// 写入文件到插件文件夹
		/// </summary>
		/// <param name="plugin">插件名称</param>
		/// <param name="path">路径</param>
		/// <param name="contents">内容</param>
		/// <param name="pluginDirectoryIndex">使用的插件目录的序号</param>
		public void WritePluginFile(
			string plugin, string path, string contents, int pluginDirectoryIndex = 0) {
			var pathManager = Application.Ioc.Resolve<PathManager>();
			var pluginDirectories = pathManager.GetPluginDirectories();
			var fullPath = Path.Combine(pluginDirectories[pluginDirectoryIndex], plugin, path);
			PathUtils.EnsureParentDirectory(fullPath);
			File.WriteAllText(fullPath, contents);
			CleanupPaths.Add(fullPath);
		}

		/// <summary>
		/// 写入文件到AppData
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="contents">内容</param>
		public void WriteAppDataFile(string path, string contents) {
			var pathConfig = Application.Ioc.Resolve<PathConfig>();
			var fullPath = Path.Combine(pathConfig.AppDataDirectory, path);
			PathUtils.EnsureParentDirectory(fullPath);
			File.WriteAllText(fullPath, contents);
			CleanupPaths.Add(fullPath);
		}

		/// <summary>
		/// 释放对容器的重载和删除写入的文件和文件夹
		/// </summary>
		public void Dispose() {
			OverrideIoc.Dispose();
			foreach (var path in CleanupPaths) {
				if (File.Exists(path)) {
					File.Delete(path);
				} else if (Directory.Exists(path)) {
					Directory.Delete(path, true);
				}
			}
		}
	}
}
