using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ZKWeb.Toolkits.WebsitePublisher.Model;
using ZKWeb.Toolkits.WebsitePublisher.Properties;
using ZKWeb.Toolkits.WebsitePublisher.Utils;

namespace ZKWeb.Toolkits.WebsitePublisher {
	/// <summary>
	/// Website publisher
	/// </summary>
	public class WebsitePublisher {
		/// <summary>
		/// Publish website parameters
		/// </summary>
		public PublishWebsiteParameters Parameters { get; protected set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="parameters">Publish website parameters</param>
		public WebsitePublisher(PublishWebsiteParameters parameters) {
			parameters.Check();
			Parameters = parameters;
		}

		/// <summary>
		/// Get directory of website root
		/// </summary>
		/// <returns></returns>
		protected virtual string GetWebRoot() {
			return Path.GetFullPath(Parameters.WebRoot);
		}

		/// <summary>
		/// Get path of `Web.config`
		/// </summary>
		/// <returns></returns>
		protected virtual string GetWebConfigPath() {
			var webRoot = GetWebRoot();
			var webConfigPath = Path.Combine(webRoot, "Web.config");
			if (!File.Exists(webConfigPath)) {
				webConfigPath = Path.Combine(webRoot, "web.config"); // 照顾到大小写区分的文件系统
			}
			if (!File.Exists(webConfigPath)) {
				throw new FileNotFoundException(Resources.WebConfigNotFound);
			}
			return webConfigPath;
		}

		/// <summary>
		/// Get directory of bin
		/// Asp.Net:
		/// - Use WebRoot\bin
		/// Asp.Net Core:
		/// - Find directory contains "ZKWeb.dll", "release", framework
		/// - Publish with .Net Core is not support yet
		/// </summary>
		/// <param name="isCore">Is Asp.Net Core</param>
		/// <returns></returns>
		protected virtual string GetBinDirectory(out bool isCore) {
			var webRoot = GetWebRoot();
			var binDir = Path.Combine(webRoot, "bin");
			if (!File.Exists(Path.Combine(binDir, "ZKWeb.dll"))) {
				isCore = true;
				var configuration = Parameters.Configuration.ToLower();
				var framework = Parameters.Framework.ToLower();
				var dllPaths = Directory.EnumerateFiles(binDir, "ZKWeb.dll", SearchOption.AllDirectories)
					.Where(p => {
						var relPath = p.Substring(webRoot.Length).ToLower();
						return relPath.Contains(configuration) && relPath.Contains(framework);
					}).ToList();
				// prefer directory not contains publish
				var dllPath = dllPaths.FirstOrDefault(d => !d.Contains("publish"));
				if (dllPath == null) {
					dllPath = dllPaths.FirstOrDefault();
				}
				if (dllPath == null) {
					throw new DirectoryNotFoundException(Resources.BinDirectoryNotFound);
				}
				binDir = Path.GetDirectoryName(dllPath);
			} else {
				isCore = false;
			}
			return binDir;
		}

		/// <summary>
		/// Get path of `config.json`
		/// </summary>
		/// <returns></returns>
		protected virtual string GetConfigJsonPath() {
			var webRoot = GetWebRoot();
			return Path.Combine(webRoot, "App_Data", "config.json");
		}

		/// <summary>
		/// Get Asp.Net Core launcher path
		/// </summary>
		/// <param name="binDir">bin directory</param>
		/// <returns></returns>
		protected virtual string GetAspNetCoreLauncherPath(string binDir) {
			var exeName = Directory
				.EnumerateFiles(binDir, "*.exe", SearchOption.TopDirectoryOnly)
				.Select(path => Path.GetFileName(path))
				.Where(name => !name.Contains(".vshost.")).FirstOrDefault();
			if (string.IsNullOrEmpty(exeName)) {
				exeName = Directory
					.EnumerateFiles(binDir, "*.dll", SearchOption.TopDirectoryOnly)
					.Select(path => path.Substring(0, path.Length - ".dll".Length))
					.Where(path => File.Exists(path)).FirstOrDefault();
			}
			if (string.IsNullOrEmpty(exeName)) {
				throw new FileNotFoundException(Resources.AspNetCoreLauncherExeNotFound);
			}
			return "." + Path.DirectorySeparatorChar + exeName;
		}

		/// <summary>
		/// Find plugin path
		/// </summary>
		/// <param name="config">Website configuration</param>
		/// <param name="pluginName">Plugin name</param>
		/// <returns></returns>
		protected virtual string FindPluginDirectory(WebsiteConfig config, string pluginName) {
			var pluginDirectories = config.PluginDirectories
				.Select(d => Path.GetFullPath(Path.Combine(GetWebRoot(), d))).ToList();
			foreach (var dir in pluginDirectories) {
				var pluginDir = Path.Combine(dir, pluginName);
				if (Directory.Exists(pluginDir)) {
					return pluginDir;
				}
			}
			throw new DirectoryNotFoundException(
				string.Format(Resources.PluginDirectoryFor_0_NotFound, pluginName));
		}

		/// <summary>
		/// Publish website
		/// </summary>
		public virtual void PublishWebsite() {
			// Get paths
			var webRoot = GetWebRoot();
			var webConfigPath = GetWebConfigPath();
			var isCore = false;
			var binDir = GetBinDirectory(out isCore);
			var configJsonPath = GetConfigJsonPath();
			var outputDir = Path.Combine(Parameters.OutputDirectory, Parameters.OutputName);
			// Remove App_Data under bin directory, because `dotnet publish` may copy this directory
			var appDataDirToRemove = Path.Combine(binDir, "App_Data");
			if (Directory.Exists(appDataDirToRemove)) {
				Directory.Delete(appDataDirToRemove, true);
			}
			// Copy website binaries
			var ignorePattern = string.IsNullOrEmpty(Parameters.IgnorePattern) ?
				null : new Regex(Parameters.IgnorePattern);
			if (!isCore) {
				// Asp.Net: copy files to output\bin, and copy Global.asax
				DirectoryUtils.CopyDirectory(
					binDir, Path.Combine(outputDir, "bin"), ignorePattern);
				File.Copy(webConfigPath, Path.Combine(outputDir, "web.config"), true);
				File.Copy(Path.Combine(webRoot, "Global.asax"),
					Path.Combine(outputDir, "Global.asax"), true);
			} else {
				// Asp.Net Core: copy files to output\, and replace launcher path in web.config
				DirectoryUtils.CopyDirectory(binDir, outputDir, ignorePattern);
				var webConfig = File.ReadAllText(webConfigPath);
				webConfig = webConfig.Replace("%LAUNCHER_PATH%", GetAspNetCoreLauncherPath(binDir));
				webConfig = webConfig.Replace("%LAUNCHER_ARGS%", "");
				File.WriteAllText(Path.Combine(outputDir, "web.config"), webConfig);
			}
			// Merge website configuration
			var outputConfigJsonPath = Path.Combine(outputDir, "App_Data", "config.json");
			var config = WebsiteConfig.Merge(configJsonPath, outputConfigJsonPath);
			config.PluginDirectories = new[] { "App_Data/Plugins" };
			Directory.CreateDirectory(Path.GetDirectoryName(outputConfigJsonPath));
			File.WriteAllText(outputConfigJsonPath,
				JsonConvert.SerializeObject(config, Formatting.Indented));
			// Copy plugins
			var originalConfig = WebsiteConfig.FromFile(configJsonPath);
			var outputPluginRoot = Path.Combine(outputDir, config.PluginDirectories[0]);
			foreach (var pluginName in config.Plugins) {
				var pluginDir = FindPluginDirectory(originalConfig, pluginName);
				var outputPluginDir = Path.Combine(outputPluginRoot, pluginName);
				DirectoryUtils.CopyDirectory(pluginDir, outputPluginDir, ignorePattern);
				// Remove src directory under plugin
				var srcDirectory = Path.Combine(outputPluginDir, "src");
				if (Directory.Exists(srcDirectory)) {
					Directory.Delete(srcDirectory, true);
				}
			}
		}
	}
}
