using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZKWeb.Server {
	/// <summary>
	/// Website configuration<br/>
	/// 网站配置<br/>
	/// </summary>
	public class WebsiteConfig {
		/// <summary>
		/// Object relational mapper<br/>
		/// It will load the mapper assembly named "ZKWeb.ORM.{thisValue}"<br/>
		/// eg: NHibernate, InMemory<br/>
		/// ORM的名称<br/>
		/// 它会加载名称为"ZKWeb.ORM.{ORM名称}"的程序集<br/>
		/// 例如: NHibernate, InMemory<br/>
		/// </summary>
		public string ORM { get; set; }
		/// <summary>
		/// Database name<br/>
		/// eg: PostgreSQL, SQLite, MySQL, MSSQL, MongoDB<br/>
		/// 数据库名称<br/>
		/// 例如: PostgreSQL, SQLite, MySQL, MSSQL, MongoDB<br/>
		/// </summary>
		public string Database { get; set; }
		/// <summary>
		/// Database connection string<br/>
		/// 数据库连接字符串<br/>
		/// </summary>
		public string ConnectionString { get; set; }
		/// <summary>
		/// Directories use to find plugins<br/>
		/// It should be a relative path to website root<br/>
		/// If not specified "App_Data/Plugins" will be used<br/>
		/// 用于查找插件的目录列表<br/>
		/// 它应该基于网站根目录的相对路径<br/>
		/// 如果不指定则使用"App_Data/Plugins"<br/>
		/// </summary>
		public IList<string> PluginDirectories { get; set; }
		/// <summary>
		/// Plugin names in the loading order<br/>
		/// 插件名称列表, 以加载顺序排列<br/>
		/// </summary>
		public IList<string> Plugins { get; set; }
		/// <summary>
		/// Other extra configuration<br/>
		/// 其他附加配置<br/>
		/// </summary>
		public IDictionary<string, object> Extra { get; set; }

		/// <summary>
		/// Read website configuration from path<br/>
		/// 从指定路径读取网站配置<br/>
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
