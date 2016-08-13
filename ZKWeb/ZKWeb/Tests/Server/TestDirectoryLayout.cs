using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using ZKWeb.Plugin;
using ZKWeb.Server;
using ZKWebStandard.Ioc;
using ZKWebStandard.Utils;

namespace ZKWeb.Tests.Server {
	class TestDirectoryLayout : IDisposable {
		private IDisposable OverrideIoc { get; set; }
		private WebsiteConfig WebsiteConfig { get; set; }
		public IList<string> CleanupPaths { get; private set; }

		/// <summary>
		/// Override plugin directories for testing
		/// It will remove all test files when disposed
		/// </summary>
		/// <param name="pluginDirectories">Plugin directories, default is [ "App_Data/__TestPlugins" ]</param>
		/// <param name="plugins">Plugins, default is [ "PluginA", "PluginB" ]</param>
		/// <param name="extra">Extra data, default is empty</param>
		/// <param name="pluginDirectoryIndex">Which plugin directory will use to create plugins</param>
		public TestDirectoryLayout(
			IList<string> pluginDirectories = null,
			IList<string> plugins = null,
			IDictionary<string, object> extra = null,
			int pluginDirectoryIndex = 0) {
			OverrideIoc = Application.OverrideIoc();
			WebsiteConfig = new WebsiteConfig() {
				PluginDirectories = pluginDirectories ?? new List<string>() { "App_Data/__TestPlugins" },
				Plugins = plugins ?? new List<string>() { "PluginA", "PluginB" },
				Extra = extra ?? new Dictionary<string, object>()
			};
			CleanupPaths = new List<string>();
			// Mock ConfigManager, PathConfig, PathManager, PluginManager
			var configManagerMock = Substitute.ForPartsOf<ConfigManager>();
			configManagerMock.WebsiteConfig.Returns(WebsiteConfig);
			Application.Ioc.Unregister<ConfigManager>();
			Application.Ioc.Unregister<PathManager>();
			Application.Ioc.Unregister<PluginManager>();
			Application.Ioc.RegisterInstance(configManagerMock);
			Application.Ioc.RegisterMany<PathManager>(ReuseType.Singleton);
			Application.Ioc.RegisterMany<PluginManager>(ReuseType.Singleton);
			// Create plugin directories and plugins
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
		/// Write file to plugin
		/// </summary>
		/// <param name="plugin">Plugin name</param>
		/// <param name="path">Path</param>
		/// <param name="contents">Contents</param>
		/// <param name="pluginDirectoryIndex">Which plugin directory will use to locate plugins</param>
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
		/// Write file to App_Data
		/// </summary>
		/// <param name="path">Path</param>
		/// <param name="contents">Contents</param>
		public void WriteAppDataFile(string path, string contents) {
			var pathConfig = Application.Ioc.Resolve<PathConfig>();
			var fullPath = Path.Combine(pathConfig.AppDataDirectory, path);
			PathUtils.EnsureParentDirectory(fullPath);
			File.WriteAllText(fullPath, contents);
			CleanupPaths.Add(fullPath);
		}

		/// <summary>
		/// Finish container overrding and remove test files
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
