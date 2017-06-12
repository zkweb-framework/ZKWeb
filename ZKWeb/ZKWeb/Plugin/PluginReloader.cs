using System;
using System.IO;
using System.Linq;
using System.Threading;
using ZKWeb.Storage;
using ZKWeb.Web;
using ZKWebStandard.Extensions;

namespace ZKWeb.Plugin {
	/// <summary>
	/// Automatic reload plugins and website configuration<br/>
	/// It will determine the following files are changed then reload the website after<br/>
	/// 自动重新加载插件和网站配置<br/>
	/// 它会检测以下文件是否有改变, 并在它们改变后重启网站<br/>
	/// - {Plugin directory}/*.cs<br/>
	/// - {Plugin directory}/*.json<br/>
	/// - {Plugin directory}/*.dll<br/>
	/// - App_Data/*.json (No recursion)<br/>
	/// - App_Data/*.ddl (No recursion)<br/>
	/// </summary>
	public class PluginReloader {
		/// <summary>
		/// Is website stopping<br/>
		/// 是否正在停止网站<br/>
		/// </summary>
		internal protected static int Stopping = 0;

		/// <summary>
		/// Stop website<br/>
		/// 停止网站<br/>
		/// </summary>
		internal protected static void StopWebsite() {
			if (Interlocked.Exchange(ref Stopping, 1) == 1) {
				return;
			}
			var thread = new Thread(() => {
				// Wait requests finished, up to 3 seconds	
				int retry = 0;
				while (Application.InProgressRequests > 0 && retry <= 3000) {
					Thread.Sleep(1);
					++retry;
				}
				// Call stoppers
				var stoppers = Application.Ioc.ResolveMany<IWebsiteStopper>();
				stoppers.ForEach(s => s.StopWebsite());
			});
			thread.IsBackground = true;
			thread.Start();
		}

		/// <summary>
		/// Start reloader<br/>
		/// 启动重加载器<br/>
		/// </summary>
		internal protected virtual void Start() {
			// Function use to handle file changed
			Action<string> onFileChanged = (path) => {
				var ext = Path.GetExtension(path).ToLower();
				if (ext == ".cs" || ext == ".json" || ext == ".dll") {
					StopWebsite();
				}
			};
			// Function use to start file system watcher
			Action<FileSystemWatcher> startWatcher = (watcher) => {
				watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
				watcher.Changed += (sender, e) => onFileChanged(e.FullPath);
				watcher.Created += (sender, e) => onFileChanged(e.FullPath);
				watcher.Deleted += (sender, e) => onFileChanged(e.FullPath);
				watcher.Renamed += (sender, e) => { onFileChanged(e.FullPath); onFileChanged(e.OldFullPath); };
				watcher.EnableRaisingEvents = true;
			};
			// Monitor plugin directory
			var pathManager = Application.Ioc.Resolve<LocalPathManager>();
			pathManager.GetPluginDirectories().Where(p => Directory.Exists(p)).ForEach(p => {
				var pluginFilesWatcher = new FileSystemWatcher();
				pluginFilesWatcher.Path = p;
				pluginFilesWatcher.IncludeSubdirectories = true;
				startWatcher(pluginFilesWatcher);
			});
			// Monitor App_Data directory
			var pathConfig = Application.Ioc.Resolve<LocalPathConfig>();
			var websiteConfigWatcher = new FileSystemWatcher();
			websiteConfigWatcher.Path = pathConfig.AppDataDirectory;
			websiteConfigWatcher.Filter = "*.json";
			startWatcher(websiteConfigWatcher);
			// Monitor ddl script files, only trigger when deleted
			var ddlWatcher = new FileSystemWatcher();
			ddlWatcher.Path = pathConfig.AppDataDirectory;
			ddlWatcher.Filter = "*.ddl";
			ddlWatcher.Deleted += (sender, e) => StopWebsite();
			ddlWatcher.EnableRaisingEvents = true;
		}
	}
}
