using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZKWeb.Web;
using ZKWeb.Web.ActionResults;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;
using ZKWebStandard.Web;
using ZKWebStandard.Web.Mock;

namespace ZKWeb.Tests.Web {
	[Tests]
	class ControllerManagerTest {
		public void OnRequest() {
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<ControllerManager>();
				Application.Ioc.RegisterMany<ControllerManager>();
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				controllerManager.RegisterController(new TestController());

				using (HttpManager.OverrideContext("__test_action_a", HttpMethods.GET)) {
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					controllerManager.OnRequest();
					Assert.Equals(response.ContentType, "text/plain");
					Assert.Equals(response.GetContentsFromBody(), "test action a");
				}

				using (HttpManager.OverrideContext("__test_action_b", HttpMethods.POST)) {
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					controllerManager.OnRequest();
					Assert.Equals(response.ContentType, "application/json");
					Assert.Equals(
						response.GetContentsFromBody(),
						JsonConvert.SerializeObject(new { a = 1 }));
				}

				// test get parameter from key
				using (HttpManager.OverrideContext("__test_action_c?name=john&age=50", HttpMethods.GET)) {
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					controllerManager.OnRequest();
					Assert.Equals(response.ContentType, "application/json");
					var json = response.GetContentsFromBody();
					var obj = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
					Assert.Equals(obj.GetOrDefault<string>("name"), "john");
					Assert.Equals(obj.GetOrDefault<int>("age"), 50);
				}

				// test get all parameters from form
				using (HttpManager.OverrideContext("__test_action_d?name=john&age=50", HttpMethods.POST)) {
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					controllerManager.OnRequest();
					Assert.Equals(response.ContentType, "application/json");
					var json = response.GetContentsFromBody();
					var obj = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
					Assert.Equals(obj.GetOrDefault<string>("name"), "john");
					Assert.Equals(obj.GetOrDefault<int>("age"), 50);
				}

				// test get all parameters from json body
				using (HttpManager.OverrideContext("__test_action_e", HttpMethods.POST)) {
					var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					request.contentType = "application/json";
					request.body = new MemoryStream(Encoding.UTF8.GetBytes(
						JsonConvert.SerializeObject(new { name = "john", age = 50 })));
					controllerManager.OnRequest();
					Assert.Equals(response.ContentType, "application/json");
					var json = response.GetContentsFromBody();
					var obj = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
					Assert.Equals(obj.GetOrDefault<string>("name"), "john");
					Assert.Equals(obj.GetOrDefault<int>("age"), 50);
				}
			}
		}

		public void RegisterController() {
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<ControllerManager>();
				Application.Ioc.RegisterMany<ControllerManager>();
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				controllerManager.RegisterController(new TestController());
				Assert.Equals(
					((PlainResult)controllerManager.GetAction("__test_action_a", HttpMethods.GET)()).Text,
					"test action a");
				Assert.Equals(
					((PlainResult)controllerManager.GetAction("__test_action_a", HttpMethods.POST)()).Text,
					"test action a");
				Assert.Equals(controllerManager.GetAction("__test_action_b", HttpMethods.GET), null);
				var obj = ((JsonResult)controllerManager
					.GetAction("__test_action_b", HttpMethods.POST)()).Object;
				Assert.Equals(JsonConvert.SerializeObject(obj), JsonConvert.SerializeObject(new { a = 1 }));
			}
		}

		public void NormalizePath() {
			var controllerManager = Application.Ioc.Resolve<ControllerManager>();
			Assert.Equals(controllerManager.NormalizePath("abc"), "/abc");
			Assert.Equals(controllerManager.NormalizePath("/abc/"), "/abc");
			Assert.Equals(controllerManager.NormalizePath("/"), "/");
		}

		public void RegisterAction() {
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<ControllerManager>();
				Application.Ioc.RegisterMany<ControllerManager>();
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				var actionGet = new Func<IActionResult>(() => { return new PlainResult("get"); });
				var actionPost = new Func<IActionResult>(() => { return new PlainResult("post"); });
				var actionOther = new Func<IActionResult>(() => { return new PlainResult("other"); });

				controllerManager.RegisterAction("__test_action", HttpMethods.GET, actionGet);
				controllerManager.RegisterAction("__test_action", HttpMethods.POST, actionPost);
				Assert.Throws<ArgumentException>(() =>
					controllerManager.RegisterAction("__test_action", HttpMethods.GET, actionOther));

				Assert.Equals(controllerManager.GetAction("__test_action", HttpMethods.GET), actionGet);
				Assert.Equals(controllerManager.GetAction("__test_action", HttpMethods.POST), actionPost);

				controllerManager.RegisterAction("__test_action", HttpMethods.GET, actionOther, true);
				Assert.Equals(controllerManager.GetAction("__test_action", HttpMethods.GET), actionOther);
				Assert.Equals(controllerManager.GetAction("__test_action", HttpMethods.POST), actionPost);
			}
		}

		public void UnregisterAction() {
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<ControllerManager>();
				Application.Ioc.RegisterMany<ControllerManager>();
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				var actionGet = new Func<IActionResult>(() => { return new PlainResult("get"); });

				controllerManager.RegisterAction("__test_action", HttpMethods.GET, actionGet);
				Assert.Equals(controllerManager.GetAction("__test_action", HttpMethods.GET), actionGet);
				Assert.IsTrue(controllerManager.UnregisterAction("__test_action", HttpMethods.GET));
				Assert.IsTrue(!controllerManager.UnregisterAction("__test_action", HttpMethods.GET));
				Assert.Equals(controllerManager.GetAction("__test_action", HttpMethods.GET), null);
			}
		}

		public void GetAction() {
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<ControllerManager>();
				Application.Ioc.RegisterMany<ControllerManager>();
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				var actionGet = new Func<IActionResult>(() => { return new PlainResult("get"); });
				var actionPost = new Func<IActionResult>(() => { return new PlainResult("post"); });

				controllerManager.RegisterAction("__test_action", HttpMethods.GET, actionGet);
				controllerManager.RegisterAction("__test_action", HttpMethods.POST, actionPost);

				Assert.Equals(controllerManager.GetAction("__test_action", HttpMethods.GET), actionGet);
				Assert.Equals(controllerManager.GetAction("__test_action", HttpMethods.POST), actionPost);
				Assert.Equals(controllerManager.GetAction("__not_exist", HttpMethods.GET), null);
			}
		}

		public class TestController : IController {
			[Action("__test_action_a")]
			[Action("__test_action_a", HttpMethods.POST)]
			public string TestActionA() {
				return "test action a";
			}

			[Action("__test_action_b", HttpMethods.POST)]
			public IActionResult TestActionB() {
				return new JsonResult(new { a = 1 });
			}

			[Action("__test_action_c")]
			public object TestActionC(string name, int age) {
				return new { name, age };
			}

			[Action("__test_action_d", HttpMethods.POST)]
			public object TestActionD(ActionParams param) {
				return new { name = param.name, age = param.age };
			}

			[Action("__test_action_e", HttpMethods.POST)]
			public object TestActionE(IDictionary<string, object> param) {
				var name = param.GetOrDefault<string>("name");
				var age = param.GetOrDefault<int>("age");
				return new { name, age };
			}
		}

		public class ActionParams {
			public string name { get; set; }
			public int age { get; set; }
		}
	}
}
