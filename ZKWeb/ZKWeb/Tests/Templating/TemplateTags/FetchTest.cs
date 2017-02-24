using DotLiquid;
using ZKWebStandard.Extensions;
using ZKWeb.Web;
using ZKWeb.Web.ActionResults;
using ZKWebStandard.Testing;
using ZKWebStandard.Web;
using ZKWebStandard.Ioc;
using ZKWeb.Templating;

namespace ZKWeb.Tests.Templating.TemplateTags {
	[Tests]
	class FetchTest {
		public void Render() {
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<ControllerManager>();
				Application.Ioc.RegisterMany<ControllerManager>(ReuseType.Singleton);
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				controllerManager.RegisterAction("__test_path", "POST", () => {
					var request = HttpManager.CurrentContext.Request;
					var id = request.Get<long>("id");
					var name = request.Get<string>("name");
					var age = request.Get<long>("age");
					return new JsonResult(new { id, name, age });
				});
				using (HttpManager.OverrideContext("/test?age=108", "GET")) {
					var templateManager = Application.Ioc.Resolve<TemplateManager>();
					var result = Template.Parse(
						"{% fetch /__test_path?id=123&name={name}&age={age} > info %}" +
						"{{ info.id }} {{ info.name }} {{ info.age }}")
						.Render(templateManager.CreateHash(new { name = "test name" }));
					Assert.Equals(result, "123 test name 108");
				}
			}
		}
	}
}
