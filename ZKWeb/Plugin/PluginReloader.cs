using System;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Server;
using ZKWebStandard.Extensions;
using ZKWebStandard.Ioc;

namespace ZKWeb.Plugin {
	/// <summary>
	/// 自动重新载入插件和网站配置
	/// 检测网站目录的以下文件是否有改变，有改变时卸载当前程序域来让下次打开网站时重新载入
	/// - 插件根目录/*.cs
	/// - 插件根目录/*.json
	/// - 插件根目录/*.dll
	/// - App_Data/*.json (仅根目录)
	/// - App_Data/DatabaseScript.txt (仅删除)
	/// </summary>
	internal static class PluginReloader {
		/// <summary>
		/// 停止网站
		/// </summary>
		internal static void StopWebsite() {
			HttpRuntime.UnloadAppDomain();
		}

		/// <summary>
		/// 启用自动重新载入插件和网站配置
		/// </summary>
		internal static void Start() {
			// 指定文件改变时卸载程序域
			Action<string> onFileChanged = (path) => {
				var ext = Path.GetExtension(path).ToLower();
				if (ext == ".cs" || ext == ".json" || ext == ".dll") {
					StopWebsite();
				}
			};
			// 绑定文件监视器的事件处理函数并启用监视器
			Action<FileSystemWatcher> startWatcher = (watcher) => {
				watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
				watcher.Changed += (sender, e) => onFileChanged(e.FullPath);
				watcher.Created += (sender, e) => onFileChanged(e.FullPath);
				watcher.Deleted += (sender, e) => onFileChanged(e.FullPath);
				watcher.Renamed += (sender, e) => { onFileChanged(e.FullPath); onFileChanged(e.OldFullPath); };
				watcher.EnableRaisingEvents = true;
			};
			// 监视插件目录
			var pathManager = Application.Ioc.Resolve<PathManager>();
			pathManager.GetPluginDirectories().Where(p => Directory.Exists(p)).ForEach(p => {
				var pluginFilesWatcher = new FileSystemWatcher();
				pluginFilesWatcher.Path = p;
				pluginFilesWatcher.IncludeSubdirectories = true;
				startWatcher(pluginFilesWatcher);
			});
			// 监视网站配置文件
			var pathConfig = Application.Ioc.Resolve<PathConfig>();
			var websiteConfigWatcher = new FileSystemWatcher();
			websiteConfigWatcher.Path = pathConfig.AppDataDirectory;
			websiteConfigWatcher.Filter = "*.json";
			startWatcher(websiteConfigWatcher);
			// 监视DatabaseScriptPath，仅监视删除
			var databaseScriptWatcher = new FileSystemWatcher();
			databaseScriptWatcher.Path = Path.GetDirectoryName(pathConfig.DatabaseScriptPath);
			databaseScriptWatcher.Filter = Path.GetFileName(pathConfig.DatabaseScriptPath);
			databaseScriptWatcher.Deleted += (sender, e) => StopWebsite();
			databaseScriptWatcher.EnableRaisingEvents = true;
		}
	}
}
