using DotLiquid;
using ZKWeb.Templating.DynamicContents;
using ZKWeb.Tests.Storage;
using ZKWebStandard.Ioc;
using ZKWebStandard.Testing;
using ZKWebStandard.Web;

namespace ZKWeb.Tests.Templating.DynamicContents {
	[Tests]
	class TemplateAreaManagerTest {
		public void All() {
			using (var layout = new TestDirectoryLayout()) {
				Application.Ioc.Unregister<TemplateAreaManager>();
				Application.Ioc.RegisterMany<TemplateAreaManager>(ReuseType.Singleton);
				var areaManager = Application.Ioc.Resolve<TemplateAreaManager>();

				areaManager.GetArea("__test_area").DefaultWidgets.Add("__test");
				Assert.Equals(areaManager.GetArea("__test_area").DefaultWidgets[0].Path, "__test");

				layout.WritePluginFile("PluginA", "templates/__test.widget",
					"{ Name: 'TestWidget', CacheTime: 123, CacheBy: 'Url' }");
				layout.WritePluginFile("PluginA", "templates/__test.html", "test contents {{ name }}");
				var widgetInfo = areaManager.GetWidgetInfo("__test");
				Assert.Equals(widgetInfo.Name, "TestWidget");
				Assert.Equals(widgetInfo.CacheTime, 123);
				Assert.Equals(widgetInfo.CacheBy, "Url");

				Assert.Equals(areaManager.GetCustomWidgets("__test_area"), null);
				areaManager.SetCustomWidgets("__test_area", new[] {
					new TemplateWidget("__custom_test")
				});
				var customWidgets = areaManager.GetCustomWidgets("__test_area");
				Assert.Equals(customWidgets.Count, 1);
				Assert.Equals(customWidgets[0].Path, "__custom_test");
				areaManager.SetCustomWidgets("__test_area", null);
				Assert.Equals(areaManager.GetCustomWidgets("__test_area"), null);

				using (HttpManager.OverrideContext("/a", "GET")) {
					var context = new Context();
					context["name"] = "a";
					var contents = areaManager.RenderWidget(context, new TemplateWidget("__test"));
					Assert.IsTrueWith(contents.Contains("test contents a"), contents);
				}

				using (HttpManager.OverrideContext("/b", "GET")) {
					var context = new Context();
					context["name"] = "b";
					var contents = areaManager.RenderWidget(context, new TemplateWidget("__test"));
					Assert.IsTrueWith(contents.Contains("test contents b"), contents);
				}
			}
		}
	}
}
