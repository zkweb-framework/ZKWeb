using ZKWeb.Storage;

namespace ZKWeb.Server {
	/// <summary>
	/// Website config manager<br/>
	/// 网站配置管理器<br/>
	/// </summary>
	/// <example>
	/// <code language="cs">
	/// var websiteConfigManager = Application.Ioc.Resolve&lt;WebsiteConfigManager&gt;();
	/// var config = websiteConfigManager.WebsiteConfig();
	/// </code>
	/// </example>
	public class WebsiteConfigManager {
		/// <summary>
		/// Website configuration<br/>
		/// 网站配置<br/>
		/// </summary>
		public virtual WebsiteConfig WebsiteConfig { get; protected internal set; }

		/// <summary>
		/// Load website configuration<br/>
		/// 加载网站配置<br/>
		/// </summary>
		internal protected virtual void Initialize() {
			var pathConfig = Application.Ioc.Resolve<LocalPathConfig>();
			WebsiteConfig = WebsiteConfig.FromFile(pathConfig.WebsiteConfigPath);
		}
	}
}
