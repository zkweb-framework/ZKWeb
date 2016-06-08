using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ZKWeb.Server {
	/// <summary>
	/// 网站配置管理器
	/// </summary>
	public class ConfigManager {
		/// <summary>
		/// 网站配置
		/// </summary>
		public virtual WebsiteConfig WebsiteConfig { get; protected set; }

		/// <summary>
		/// 载入网站配置
		/// </summary>
		internal static void Initialize() {
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			var pathConfig = Application.Ioc.Resolve<PathConfig>();
			configManager.WebsiteConfig = WebsiteConfig.FromFile(pathConfig.WebsiteConfigPath);
		}
	}
}
