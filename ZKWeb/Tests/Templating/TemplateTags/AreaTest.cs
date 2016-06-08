using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Templating.AreaSupport;
using ZKWeb.Tests.Server;
using ZKWeb.Utils.IocContainer;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Tests.Templating.TemplateTags {
	[UnitTest]
	class AreaTest {
		public void Render() {
			using (var layout = new TestDirectoryLayout()) {
				Application.Ioc.Unregister<TemplateAreaManager>();
				Application.Ioc.RegisterMany<TemplateAreaManager>(ReuseType.Singleton);
				var areaManager = Application.Ioc.Resolve<TemplateAreaManager>();

				areaManager.GetArea("__test_area").DefaultWidgets.Add("__test_a");
				areaManager.GetArea("__test_area").DefaultWidgets.Add("__test_b", new { a = 1 });

				layout.WritePluginFile("PluginA", "templates/__test_a.widget", "{}");
				layout.WritePluginFile("PluginA", "templates/__test_a.html", "widget test_a");
				layout.WritePluginFile("PluginB", "templates/__test_b.widget", "{}");
				layout.WritePluginFile("PluginB", "templates/__test_b.html", "widget test_b {{ a }}");

				var result = Template.Parse("{% area __test_area %}").Render();
				Assert.Equals(result,
					"<div class='template_area' area_id='__test_area'>" +
					"<div class='template_widget' data-widget=''>widget test_a</div>" +
					"<div class='template_widget' data-widget=''>widget test_b 1</div>" +
					"</div>");

				result = Template.Parse("{% area __test_empty_area %}").Render();
				Assert.Equals(result,
					"<div class='template_area' area_id='__test_empty_area'></div>");
			}
		}
	}
}
