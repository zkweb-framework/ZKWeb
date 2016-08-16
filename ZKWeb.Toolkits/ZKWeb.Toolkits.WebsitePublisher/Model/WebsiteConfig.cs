using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ZKWeb.Toolkits.WebsitePublisher.Model {
	/// <summary>
	/// Website configuration
	/// Deserialize from `config.json`
	/// </summary>
	public class WebsiteConfig {
		/// <summary>
		/// ORM
		/// </summary>
		public string ORM { get; set; }
		/// <summary>
		/// Database
		/// </summary>
		public string Database { get; set; }
		/// <summary>
		/// Connection string
		/// </summary>
		public string ConnectionString { get; set; }
		/// <summary>
		/// Plugin directories
		/// </summary>
		public IList<string> PluginDirectories { get; set; }
		/// <summary>
		/// Plugins
		/// </summary>
		public IList<string> Plugins { get; set; }
		/// <summary>
		/// Extra data
		/// </summary>
		public IDictionary<string, object> Extra { get; set; }

		/// <summary>
		/// Read configuration from path
		/// </summary>
		/// <param name="path">Path of config.json</param>
		/// <returns></returns>
		public static WebsiteConfig FromFile(string path) {
			var json = File.ReadAllText(path);
			var config = JsonConvert.DeserializeObject<WebsiteConfig>(json);
			return config;
		}

		/// <summary>
		/// Merge configuration
		/// If toPath exists, then only replace plugins list
		/// </summary>
		/// <param name="fromPath">From path</param>
		/// <param name="toPath">To path</param>
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
