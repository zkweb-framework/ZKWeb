using ZKWeb.Storage;

namespace ZKWeb.Server {
	/// <summary>
	/// Website config manager
	/// </summary>
	public class WebsiteConfigManager {
		/// <summary>
		/// Website configuration
		/// </summary>
		public virtual WebsiteConfig WebsiteConfig { get; protected set; }

		/// <summary>
		/// Load website configuration
		/// </summary>
		internal static void Initialize() {
			var configManager = Application.Ioc.Resolve<WebsiteConfigManager>();
			var pathConfig = Application.Ioc.Resolve<LocalPathConfig>();
			configManager.WebsiteConfig = WebsiteConfig.FromFile(pathConfig.WebsiteConfigPath);
		}
	}
}
