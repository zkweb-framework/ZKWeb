using System.Collections.Generic;
using System.IO;
using ZKWeb.Templating;
using ZKWeb.Tests.Storage;
using ZKWebStandard.Ioc;
using ZKWebStandard.Testing;

namespace ZKWeb.Tests.Templating {
	[Tests]
	class TemplateManagerTest {
		public void NewHash() {
			var templateManager = Application.Ioc.Resolve<TemplateManager>();
			var hashFromNull = templateManager.CreateHash(null);
			Assert.Equals(hashFromNull.Count, 0);
			var hashFromDictionary = templateManager.CreateHash(
				new Dictionary<string, object>() { { "key", "value" } });
			Assert.Equals(hashFromDictionary["key"], "value");
			var hashFromObject = templateManager.CreateHash(new { key = "value" });
			Assert.Equals(hashFromObject["key"], "value");
		}

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
