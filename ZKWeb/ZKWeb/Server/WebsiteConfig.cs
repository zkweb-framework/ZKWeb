using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZKWeb.Server {
	/// <summary>
	/// Website configuration
	/// </summary>
	public class WebsiteConfig {
		/// <summary>
		/// Object relational mapper
		/// It will load the mapper assembly named "ZKWeb.ORM.{thisValue}"
		/// eg: NHibernate, InMemory
		/// </summary>
		public string ORM { get; set; }
		/// <summary>
		/// Database name
		/// eg: PostgreSQL, SQLite, MySQL, MSSQL
		/// </summary>
		public string Database { get; set; }
		/// <summary>
		/// Database connection string
		/// </summary>
		public string ConnectionString { get; set; }
		/// <summary>
		/// Directories use to find plugins
		/// It should be a relative path to website root
		/// If not specified "App_Data/Plugins" will be used
		/// </summary>
		public IList<string> PluginDirectories { get; set; }
		/// <summary>
		/// Plugin names in the loading order
		/// </summary>
		public IList<string> Plugins { get; set; }
		/// <summary>
		/// Other extra configuration
		/// </summary>
		public IDictionary<string, object> Extra { get; set; }

		/// <summary>
		/// Read website configuration from path
		/// </summary>
		/// <param name="path">Configuration path</param>
		/// <returns></returns>
		public static WebsiteConfig FromFile(string path) {
			var json = File.ReadAllText(path);
			var config = JsonConvert.DeserializeObject<WebsiteConfig>(json);
			config.PluginDirectories = config.PluginDirectories ?? new List<string>();
			if (!config.PluginDirectories.Any()) {
				config.PluginDirectories.Add("App_Data/Plugins");
			}
			config.Plugins = config.Plugins ?? new List<string>();
			config.Extra = config.Extra ?? new Dictionary<string, object>();
			return config;
		}
	}
}
