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
	public static class ConfigManager {
		/// <summary>
		/// 获取网站配置
		/// </summary>
		/// <returns></returns>
		public static WebsiteConfig GetWebsiteConfig() {
			var path = PathUtils.SecureCombine(PathUtils.WebRoot.Value, "App_Data", "config.json");
			var text = File.ReadAllText(path);
			return JsonConvert.DeserializeObject<WebsiteConfig>(text);
		}
	}
}
