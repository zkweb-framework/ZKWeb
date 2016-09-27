using DotLiquid;
using System.IO;
using ZKWeb.Templating;
using ZKWeb.Tests.Storage;
using ZKWebStandard.Ioc;
using ZKWebStandard.Testing;

namespace ZKWeb.Tests.Templating {
	[Tests]
	class TemplateFileSystemTest {
		public void ReadTemplateFile() {
			using (var layout = new TestDirectoryLayout()) {
				Application.Ioc.Unregister<TemplateFileSystem>();
				Application.Ioc.RegisterMany<TemplateFileSystem>(ReuseType.Singleton);
				layout.WritePluginFile("PluginA", "templates/__test_a.html", "test a");
				layout.WritePluginFile("PluginB", "templates/__test_b.html", "test b");
				var filesystem = Application.Ioc.Resolve<TemplateFileSystem>();
				var context = new Context();
				var templateA = (Template)filesystem.ReadTemplateFile(context, "__test_a.html");
				var templateB = (Template)filesystem.ReadTemplateFile(context, "__test_b.html");
				Assert.Throws<FileNotFoundException>(() =>
					filesystem.ReadTemplateFile(context, "__test_c.html"));
				Assert.IsTrue(templateA != null);
				Assert.IsTrue(templateB != null);
				Assert.Equals(templateA.Render(), "test a");
				Assert.Equals(templateB.Render(), "test b");
			}
		}
	}
}
