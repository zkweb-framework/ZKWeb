using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Server;
using ZKWeb.Utils.Functions;
using ZKWeb.Utils.IocContainer;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Tests.Server {
	[UnitTest]
	class PathManagerTest {
		public void GetPluginDirectories() {
			using (Application.OverrideIoc()) {
				var configManagerMock = new Mock<ConfigManager>();
				var webConfig = new WebsiteConfig() {
					PluginDirectories = new List<string>() { "AppData/__TestPluginsA", "AppData/__TestPluginsB" }
				};
				configManagerMock.Setup(c => c.WebsiteConfig).Returns(webConfig);
				Application.Ioc.Unregister<ConfigManager>();
				Application.Ioc.Unregister<PathManager>();
				Application.Ioc.RegisterInstance(configManagerMock.Object);
				Application.Ioc.RegisterMany<PathManager>(ReuseType.Singleton);
				var pathManager = Application.Ioc.Resolve<PathManager>();
				var pluginDirectories = pathManager.GetPluginDirectories();
				Assert.Equals(pluginDirectories.Count, 2);
				Assert.Equals(pluginDirectories[0],
					Path.Combine(PathUtils.WebRoot.Value, webConfig.PluginDirectories[0]));
				Assert.Equals(pluginDirectories[1],
					Path.Combine(PathUtils.WebRoot.Value, webConfig.PluginDirectories[1]));
			}
		}

		public void GetTemplateFullPath() {

		}

		public void GetResourceFullPath() {

		}

		public void GetStorageFullPath() {

		}
	}
}
