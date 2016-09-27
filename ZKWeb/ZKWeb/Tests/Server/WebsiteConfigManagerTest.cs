using ZKWeb.Server;
using ZKWebStandard.Testing;

namespace ZKWeb.Tests.Server {
	[Tests]
	class WebsiteConfigManagerTest {
		public void All() {
			var configManager = Application.Ioc.Resolve<WebsiteConfigManager>();
			Assert.IsTrue(configManager.WebsiteConfig != null);
		}
	}
}
