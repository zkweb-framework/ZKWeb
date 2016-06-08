using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Templating.AreaSupport;
using ZKWeb.Tests.Server;
using ZKWeb.Utils.Functions;
using ZKWeb.Utils.IocContainer;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Tests.Templating.AreaSupport {
	[UnitTest]
	class TemplateAreaManagerTest {
		public void All() {
			using (var layout = new TestDirectoryLayout()) {
				Application.Ioc.Unregister<TemplateAreaManager>();
				Application.Ioc.RegisterMany<TemplateAreaManager>(ReuseType.Singleton);
				var areaManager = Application.Ioc.Resolve<TemplateAreaManager>();

				areaManager.GetArea("__test_area").DefaultWidgets.Add("test");
				Assert.Equals(areaManager.GetArea("__test_area").DefaultWidgets[0].Path, "test");

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

				using (HttpContextUtils.OverrideContext("/a", "GET")) {
					var context = new Context();
					context["name"] = "a";
					var contents = areaManager.RenderWidget(context, new TemplateWidget("__test"));
					Assert.IsTrueWith(contents.Contains("test contents a"), contents);
				}

				using (HttpContextUtils.OverrideContext("/b", "GET")) {
					var context = new Context();
					context["name"] = "b";
					var contents = areaManager.RenderWidget(context, new TemplateWidget("__test"));
					Assert.IsTrueWith(contents.Contains("test contents b"), contents);
				}
			}
		}
	}
}
