using Newtonsoft.Json;
using System.IO;
using System.Linq;
using ZKWeb.Toolkits.WebsitePublisher.Model;
using ZKWeb.Toolkits.WebsitePublisher.Utils;

namespace ZKWeb.Toolkits.WebsitePublisher {
	/// <summary>
	/// 网站发布器
	/// </summary>
	public class WebsitePublisher {
		/// <summary>
		/// 发布网站的参数
		/// </summary>
		public PublishWebsiteParameters Parameters { get; protected set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="parameters">发布网站的参数</param>
		public WebsitePublisher(PublishWebsiteParameters parameters) {
			parameters.Check();
			Parameters = parameters;
		}

		/// <summary>
		/// 获取网站根目录
		/// </summary>
		/// <returns></returns>
		protected virtual string GetWebRoot() {
			return Path.GetFullPath(Parameters.WebRoot);
		}

		/// <summary>
		/// 获取Web.config的路径
		/// </summary>
		/// <returns></returns>
		protected virtual string GetWebConfigPath() {
			var webRoot = GetWebRoot();
			var webConfigPath = Path.Combine(webRoot, "Web.config");
			if (!File.Exists(webConfigPath)) {
				Path.Combine(Parameters.WebRoot, "web.config"); // 照顾到大小写区分的文件系统
			}
			if (!File.Exists(webConfigPath)) {
				throw new FileNotFoundException("web.config not found");
			}
			return webConfigPath;
		}

		/// <summary>
		/// 获取bin目录
		/// Asp.Net时是WebRoot\bin
		/// Asp.Net Core时需要查找
		/// </summary>
		/// <param name="isCore">是否Asp.Net Core</param>
		/// <returns></returns>
		protected virtual string GetBinDirectory(out bool isCore) {
			var webRoot = GetWebRoot();
			var binDir = Path.Combine(webRoot, "bin");
			if (!File.Exists(Path.Combine(binDir, "ZKWeb.dll"))) {
				isCore = true;
				var dllPath = Directory.EnumerateFiles(binDir, "ZKWeb.dll", SearchOption.AllDirectories)
					.Where(p => p.Substring(webRoot.Length).Contains("Release")).FirstOrDefault();
				if (dllPath == null) {
					throw new DirectoryNotFoundException("bin directory not found");
				}
				binDir = Path.GetDirectoryName(dllPath);
			} else {
				isCore = false;
			}
			return binDir;
		}

		/// <summary>
		/// 获取config.json的路径
		/// </summary>
		/// <returns></returns>
		protected virtual string GetConfigJsonPath() {
			var webRoot = GetWebRoot();
			return Path.Combine(webRoot, "App_Data", "config.json");
		}

		/// <summary>
		/// 查找插件目录
		/// </summary>
		/// <param name="config">网站配置</param>
		/// <param name="pluginName">插件名称</param>
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
			throw new DirectoryNotFoundException($"Plugin directory for {pluginName} not found");
		}

		/// <summary>
		/// 发布网站
		/// </summary>
		public virtual void PublishWebsite() {
			// 计算各路径
			var webRoot = GetWebRoot();
			var webConfigPath = GetWebConfigPath();
			var isCore = false;
			var binDir = GetBinDirectory(out isCore);
			var configJsonPath = GetConfigJsonPath();
			var outputDir = Path.Combine(Parameters.OutputDirectory, Parameters.OutputName);
			// 复制网站程序
			// Asp.Net需要复制到bin下，Asp.Net Core需要复制到根目录
			var outputBinDir = isCore ? outputDir : Path.Combine(outputDir, "bin");
			DirectoryUtils.CopyDirectory(binDir, outputBinDir);
			File.Copy(webConfigPath, Path.Combine(outputDir, "web.config"), true);
			// 整合和复制网站配置
			var outputConfigJsonPath = Path.Combine(outputDir, "App_Data", "config.json");
			var config = WebsiteConfig.Merge(configJsonPath, outputConfigJsonPath);
			config.PluginDirectories = new[] { "App_Data/Plugins" };
			Directory.CreateDirectory(Path.GetDirectoryName(outputConfigJsonPath));
			File.WriteAllText(outputConfigJsonPath,
				JsonConvert.SerializeObject(config, Formatting.Indented));
			// 复制各个插件
			var originalConfig = WebsiteConfig.FromFile(configJsonPath);
			var outputPluginRoot = Path.Combine(outputDir, config.PluginDirectories[0]);
			foreach (var pluginName in config.Plugins) {
				var pluginDir = FindPluginDirectory(originalConfig, pluginName);
				var outputPluginDir = Path.Combine(outputPluginRoot, pluginName);
				DirectoryUtils.CopyDirectory(pluginDir, outputPluginDir);
			}
			// 删除各个插件下的src目录
			foreach (var dir in Directory.EnumerateDirectories(
				outputPluginRoot, "src", SearchOption.AllDirectories)) {
				Directory.Delete(dir, true);
			}
		}
	}
}
