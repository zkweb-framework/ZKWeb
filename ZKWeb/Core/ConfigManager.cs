using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Core.Model;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Core {
	/// <summary>
	/// 配置管理器
	/// 配置文件位于
	///		App_Data/config.json
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
			Reload();
		}

		/// <summary>
		/// 重新载入所有配置
		/// </summary>
		public void Reload() {
			var path = PathUtils.SecureCombine(PathUtils.WebRoot.Value, "App_Data", "config.json");
			var text = File.ReadAllText(path);
			WebsiteConfig = JsonConvert.DeserializeObject<WebsiteConfig>(text);
		}
	}
}
