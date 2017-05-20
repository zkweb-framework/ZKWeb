using System;

namespace ZKWeb.Server {
	/// <summary>
	/// Renamed to WebsiteConfigManager<br/>
	/// 已更名为WebsiteConfigManager<br/>
	/// </summary>
	[Obsolete("Renamed to WebsiteConfigManager")]
	public class ConfigManager {
		/// <summary>
		/// Website configuration<br/>
		/// 网站配置<br/>
		/// </summary>
		public virtual WebsiteConfig WebsiteConfig {
			get { return Application.Ioc.Resolve<WebsiteConfigManager>().WebsiteConfig; }
		}
	}
}
