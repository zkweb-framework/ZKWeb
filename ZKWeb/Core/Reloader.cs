using DryIoc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using ZKWeb.Model;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Core {
	/// <summary>
	/// 自动重新载入插件和网站配置
	/// 检测网站目录的以下文件是否有改变，有改变时卸载当前程序域来让下次打开网站时重新载入
	///		插件根目录/*.cs
	///		插件根目录/*.json
	///		App_Data/*.json (仅根目录)
	///		App_Data/DatabaseScript.txt (仅删除)
	/// </summary>
	public static class Reloader {
		/// <summary>
		/// 启用重载器
		/// </summary>
		public static void Start() {
			// 指定文件改变时卸载程序域
			Action<string> onFileChanged = (path) => {
				var ext = Path.GetExtension(path).ToLower();
				if (ext == ".cs" || ext == ".json") {
					HttpRuntime.UnloadAppDomain();
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
			var websiteConfigWatcher = new FileSystemWatcher();
			websiteConfigWatcher.Path = PathConfig.AppDataDirectory;
			websiteConfigWatcher.Filter = "*.json";
			startWatcher(websiteConfigWatcher);
			// 监视DatabaseScriptPath，仅监视删除
			var databaseScriptWatcher = new FileSystemWatcher();
			databaseScriptWatcher.Path = Path.GetDirectoryName(PathConfig.DatabaseScriptPath);
			databaseScriptWatcher.Filter = Path.GetFileName(PathConfig.DatabaseScriptPath);
			databaseScriptWatcher.Deleted += (sender, e) => HttpRuntime.UnloadAppDomain();
			databaseScriptWatcher.EnableRaisingEvents = true;
		}
	}
}
