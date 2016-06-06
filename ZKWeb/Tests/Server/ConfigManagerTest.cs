using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Server;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Tests.Server {
	[UnitTest]
	class ConfigManagerTest {
		public void All() {
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			Assert.IsTrue(configManager.WebsiteConfig != null);
		}
	}
}
