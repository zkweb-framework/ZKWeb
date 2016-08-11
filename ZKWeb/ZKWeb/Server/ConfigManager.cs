namespace ZKWeb.Server {
	/// <summary>
	/// Website configuration manager
	/// </summary>
	public class ConfigManager {
		/// <summary>
		/// Website configuration
		/// </summary>
		public virtual WebsiteConfig WebsiteConfig { get; protected set; }

		/// <summary>
		/// Load website configuration
		/// </summary>
		internal static void Initialize() {
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			var pathConfig = Application.Ioc.Resolve<PathConfig>();
			configManager.WebsiteConfig = WebsiteConfig.FromFile(pathConfig.WebsiteConfigPath);
		}
	}
}
