using System;

namespace ZKWeb.Server {
	/// <summary>
	/// Renamed to WebsiteConfigManager
	/// </summary>
	[Obsolete("Renamed to WebsiteConfigManager")]
	public class ConfigManager {
		/// <summary>
		/// Website configuration
		/// </summary>
		public virtual WebsiteConfig WebsiteConfig {
			get { return Application.Ioc.Resolve<WebsiteConfigManager>().WebsiteConfig; }
		}
	}
}
