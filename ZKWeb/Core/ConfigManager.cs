using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Model;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Core {
	/// <summary>
	/// 网站配置管理器
	/// </summary>
	public class ConfigManager {
		/// <summary>
		/// 网站配置
		/// </summary>
		public WebsiteConfig WebsiteConfig { get; protected set; }

		/// <summary>
		/// 载入所有配置
		/// </summary>
		public ConfigManager() {
			WebsiteConfig = WebsiteConfig.FromFile(PathConfig.WebsiteConfigPath);
		}
	}
}
