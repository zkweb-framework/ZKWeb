using System;
using System.IO;
using System.Linq;
using ZKWeb.Server;
using ZKWeb.Web;
using ZKWebStandard.Extensions;

namespace ZKWeb.Plugin {
	/// <summary>
	/// 自动重新载入插件和网站配置
	/// 检测网站目录的以下文件是否有改变，有改变时卸载当前程序域来让下次打开网站时重新载入
	/// - 插件根目录/*.cs
	/// - 插件根目录/*.json
	/// - 插件根目录/*.dll
	/// - App_Data/*.json (仅根目录)
	/// - App_Data/*.ddl (仅删除)
	/// </summary>
	internal static class PluginReloader {
		/// <summary>
		/// 启用自动重新载入插件和网站配置
		/// </summary>
		internal static void Start() {
			// 停止网站运行的函数
			var stopWebsite = new Action(() => {
				var stoppers = Application.Ioc.ResolveMany<IWebsiteStopper>();
				stoppers.ForEach(s => s.StopWebsite());
			});
			// 文件改变时卸载程序域的函数
			Action<string> onFileChanged = (path) => {
				var ext = Path.GetExtension(path).ToLower();
				if (ext == ".cs" || ext == ".json" || ext == ".dll") {
					stopWebsite();
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
			// 监视数据库生成脚本，仅监视删除
			var ddlWatcher = new FileSystemWatcher();
			ddlWatcher.Path = pathConfig.AppDataDirectory;
			ddlWatcher.Filter = "*.ddl";
			ddlWatcher.Deleted += (sender, e) => stopWebsite();
			ddlWatcher.EnableRaisingEvents = true;
		}
	}
}
