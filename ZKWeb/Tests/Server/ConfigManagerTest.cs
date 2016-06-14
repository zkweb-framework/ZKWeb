using ZKWeb.Server;
using ZKWebStandard.Testing;

namespace ZKWeb.Tests.Server {
	[Tests]
	class ConfigManagerTest {
		public void All() {
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			Assert.IsTrue(configManager.WebsiteConfig != null);
		}
	}
}
