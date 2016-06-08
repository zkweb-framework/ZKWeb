using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.Functions;
using ZKWeb.Utils.IocContainer;
using ZKWeb.Utils.UnitTest;
using ZKWeb.Web;
using ZKWeb.Web.ActionResults;

namespace ZKWeb.Tests.Templating.TemplateTags {
	[UnitTest]
	class FetchTest {
		public void Render() {
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<ControllerManager>();
				Application.Ioc.RegisterMany<ControllerManager>(ReuseType.Singleton);
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				controllerManager.RegisterAction("__test_path", "POST", () => {
					var id = HttpContextUtils.CurrentContext.Request.Get<long>("id");
					var name = HttpContextUtils.CurrentContext.Request.Get<string>("name");
					var age = HttpContextUtils.CurrentContext.Request.Get<long>("age");
					return new JsonResult(new { id, name, age });
				});
				using (HttpContextUtils.OverrideContext("/test?age=108", "GET")) {
					var result = Template.Parse(
						"{% fetch /__test_path?id=123&name={name}&age={age} > info %}" +
						"{{ info.id }} {{ info.name }} {{ info.age }}")
						.Render(Hash.FromAnonymousObject(new { name = "test name" }));
					Assert.Equals(result, "123 test name 108");
				}
			}
		}
	}
}
