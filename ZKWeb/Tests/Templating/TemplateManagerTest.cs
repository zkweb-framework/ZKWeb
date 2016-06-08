using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Templating;
using ZKWeb.Tests.Server;
using ZKWeb.Utils.IocContainer;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Tests.Templating {
	[UnitTest]
	class TemplateManagerTest {
		public void RenderTemplate() {
			using (var layout = new TestDirectoryLayout()) {
				Application.Ioc.Unregister<TemplateManager>();
				Application.Ioc.RegisterMany<TemplateManager>(ReuseType.Singleton);
				layout.WritePluginFile("PluginA", "templates/__test_a.html", "test a {{ name }}");
				layout.WritePluginFile("PluginB", "templates/__test_b.html", "test b {{ name }}");
				var templateManager = Application.Ioc.Resolve<TemplateManager>();
				var a = templateManager.RenderTemplate("__test_a.html", new { name = "asd" });
				var b = templateManager.RenderTemplate("__test_b.html", new { name = "asd" });
				Assert.Throws<FileNotFoundException>(() =>
					templateManager.RenderTemplate("__test_c.html", new { name = "asd" }));
				Assert.Equals(a, "test a asd");
				Assert.Equals(b, "test b asd");
			}
		}
	}
}
