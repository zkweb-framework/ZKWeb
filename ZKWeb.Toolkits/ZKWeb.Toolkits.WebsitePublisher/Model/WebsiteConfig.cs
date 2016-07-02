using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ZKWeb.Toolkits.WebsitePublisher.Model {
	/// <summary>
	/// 网站配置
	/// config.json的内容
	/// </summary>
	public class WebsiteConfig {
		/// <summary>
		/// 数据库类型
		/// </summary>
		public string Database { get; set; }
		/// <summary>
		/// 连接字符串
		/// </summary>
		public string ConnectionString { get; set; }
		/// <summary>
		/// 插件根目录列表
		/// </summary>
		public IList<string> PluginDirectories { get; set; }
		/// <summary>
		/// 插件列表
		/// </summary>
		public IList<string> Plugins { get; set; }
		/// <summary>
		/// 附加数据
		/// </summary>
		public IDictionary<string, object> Extra { get; set; }

		/// <summary>
		/// 从文件读取网站配置
		/// </summary>
		/// <param name="path">config.json的路径</param>
		/// <returns></returns>
		public static WebsiteConfig FromFile(string path) {
			var json = File.ReadAllText(path);
			var config = JsonConvert.DeserializeObject<WebsiteConfig>(json);
			return config;
		}

		/// <summary>
		/// 整合网站配置
		/// 如果目标路径存在，替换插件列表，但不替换其他数据
		/// </summary>
		/// <param name="fromPath">来源路径</param>
		/// <param name="toPath">目标路径</param>
		/// <returns></returns>
		public static WebsiteConfig Merge(string fromPath, string toPath) {
			var fromConfig = FromFile(fromPath);
			var toConfig = File.Exists(toPath) ? FromFile(toPath) : fromConfig;
			toConfig.Plugins = fromConfig.Plugins ?? new List<string>();
			toConfig.PluginDirectories = toConfig.PluginDirectories ?? new List<string>();
			toConfig.Extra = toConfig.Extra ?? new Dictionary<string, object>();
			return toConfig;
		}
	}
}