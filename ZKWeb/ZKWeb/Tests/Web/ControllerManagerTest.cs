using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ZKWeb.Web;
using ZKWeb.Web.ActionResults;
using ZKWebStandard.Extensions;
using ZKWebStandard.Ioc;
using ZKWebStandard.Testing;
using ZKWebStandard.Web;
using ZKWebStandard.Web.Mock;

namespace ZKWeb.Tests.Web {
	[Tests]
	class ControllerManagerTest {
		private void OnRequestTest(
			Action action, ReuseType reuseType = ReuseType.Transient) {
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<ControllerManager>();
				Application.Ioc.RegisterMany<ControllerManager>(ReuseType.Singleton);
				Application.Ioc.Unregister<IController>();
				Application.Ioc.Register<IController, TestController>(reuseType);
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				controllerManager.Initialize();
				action();
			}
		}

		public void OnRequestTest_A() {
			OnRequestTest(() => {
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				using (HttpManager.OverrideContext("__test_action_a", HttpMethods.GET)) {
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					controllerManager.OnRequest();
					Assert.Equals(response.ContentType, "text/plain; charset=utf-8");
					Assert.Equals(response.GetContentsFromBody(), "test action a");
				}
			});
		}

		public void OnRequestTest_B() {
			OnRequestTest(() => {
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				using (HttpManager.OverrideContext("__test_action_b", HttpMethods.POST)) {
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					controllerManager.OnRequest();
					Assert.Equals(response.ContentType, "application/json; charset=utf-8");
					Assert.Equals(
						response.GetContentsFromBody(),
						JsonConvert.SerializeObject(new { a = 1 }));
				}
			});
		}

		public void OnRequestTest_C() {
			OnRequestTest(() => {
				// test get parameter from query
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				using (HttpManager.OverrideContext("__test_action_c?name=john&age=50", HttpMethods.GET)) {
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					controllerManager.OnRequest();
					Assert.Equals(response.ContentType, "application/json; charset=utf-8");
					var json = response.GetContentsFromBody();
					var obj = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
					Assert.Equals(obj.GetOrDefault<string>("name"), "john");
					Assert.Equals(obj.GetOrDefault<int>("age"), 50);
				}

				// test get parameter from json body
				using (HttpManager.OverrideContext("__test_action_c", HttpMethods.GET)) {
					var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
					request.contentType = "application/json";
					request.body = new MemoryStream(Encoding.UTF8.GetBytes("{ name: 'john', age: 50 }"));
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					controllerManager.OnRequest();
					Assert.Equals(response.ContentType, "application/json; charset=utf-8");
					var json = response.GetContentsFromBody();
					var obj = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
					Assert.Equals(obj.GetOrDefault<string>("name"), "john");
					Assert.Equals(obj.GetOrDefault<int>("age"), 50);
				}
			});
		}

		public void OnRequestTest_D() {
			OnRequestTest(() => {
				// test get all parameters from form
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				using (HttpManager.OverrideContext("__test_action_d?name=john&age=50", HttpMethods.POST)) {
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					controllerManager.OnRequest();
					Assert.Equals(response.ContentType, "application/json; charset=utf-8");
					var json = response.GetContentsFromBody();
					var obj = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
					Assert.Equals(obj.GetOrDefault<string>("name"), "john");
					Assert.Equals(obj.GetOrDefault<int>("age"), 50);
				}
			});
		}

		public void OnRequestTest_D_JsonBody() {
			OnRequestTest(() => {
				// test get complex parameter from json body
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				using (HttpManager.OverrideContext("__test_action_d", HttpMethods.POST)) {
					var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
					request.contentType = "application/json";
					request.body = new MemoryStream(Encoding.UTF8.GetBytes("{ param: { name: 'john', age: 50 } }"));
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					controllerManager.OnRequest();
					Assert.Equals(response.ContentType, "application/json; charset=utf-8");
					var json = response.GetContentsFromBody();
					var obj = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
					Assert.Equals(obj.GetOrDefault<string>("name"), "john");
					Assert.Equals(obj.GetOrDefault<int>("age"), 50);
				}
			});
		}

		public void OnRequestTest_E() {
			OnRequestTest(() => {
				// test get all parameters from json body
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				using (HttpManager.OverrideContext("__test_action_e", HttpMethods.POST)) {
					var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					request.contentType = "application/json; charset=utf-8";
					request.body = new MemoryStream(Encoding.UTF8.GetBytes(
						JsonConvert.SerializeObject(new { name = "john", age = 50 })));
					controllerManager.OnRequest();
					Assert.Equals(response.ContentType, "application/json; charset=utf-8");
					var json = response.GetContentsFromBody();
					var obj = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
					Assert.Equals(obj.GetOrDefault<string>("name"), "john");
					Assert.Equals(obj.GetOrDefault<int>("age"), 50);
				}
			});
		}

		public void OnRequestTest_F() {
			OnRequestTest(() => {
				// test get parameter from posted file
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				using (HttpManager.OverrideContext("__test_action_f", HttpMethods.POST)) {
					var file = new HttpPostFileMock() { filename = "abc.txt" };
					var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					request.postedFiles["file"] = file;
					controllerManager.OnRequest();
					Assert.Equals(response.ContentType, "application/json; charset=utf-8");
					var json = response.GetContentsFromBody();
					var obj = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
					Assert.Equals(obj.GetOrDefault<string>("filename"), "abc.txt");
				}
			});
		}

		public void OnRequestTest_G() {
			OnRequestTest(() => {
				// test action filter attribute
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				using (HttpManager.OverrideContext("__test_action_g", HttpMethods.GET)) {
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					controllerManager.OnRequest();
					var text = response.GetContentsFromBody();
					Assert.Equals(text, "GInjected");
				}
			});
		}

		public void OnRequestTest_H() {
			using (Application.OverrideIoc()) {
				// test global registered action filter
				Application.Ioc.Unregister<IActionFilter>();
				Application.Ioc.RegisterInstance<IActionFilter>(new TestActionFilter());
				OnRequestTest(() => {
					var controllerManager = Application.Ioc.Resolve<ControllerManager>();
					using (HttpManager.OverrideContext("__test_action_h", HttpMethods.GET)) {
						var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
						controllerManager.OnRequest();
						var text = response.GetContentsFromBody();
						Assert.Equals(text, "HInjected");
					}
				});
			}
		}

		public void OnRequestTest_I_Reuse() {
			OnRequestTest(() => {
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				var result = new HashSet<string>();
				for (var i = 1; i <= 3; ++i) {
					using (HttpManager.OverrideContext("__test_action_i", HttpMethods.GET)) {
						var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
						controllerManager.OnRequest();
						result.Add(response.GetContentsFromBody());
					}
				}
				Assert.Equals(result.Count, 3);
			}, ReuseType.Transient);
			OnRequestTest(() => {
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				var result = new HashSet<string>();
				for (var i = 1; i <= 3; ++i) {
					using (HttpManager.OverrideContext("__test_action_i", HttpMethods.GET)) {
						var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
						controllerManager.OnRequest();
						result.Add(response.GetContentsFromBody());
					}
				}
				Assert.Equals(result.Count, 1);
			}, ReuseType.Singleton);
		}

		public void OnRequestTest_J_Regex() {
			OnRequestTest(() => {
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				using (HttpManager.OverrideContext("__test_action_j/id_value", HttpMethods.GET)) {
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					controllerManager.OnRequest();
					Assert.Equals(response.GetContentsFromBody(), "TestActionJ_1Param_id_value");
				}
				using (HttpManager.OverrideContext("__test_action_j/id_value/extra_value", HttpMethods.GET)) {
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					controllerManager.OnRequest();
					Assert.Equals(response.GetContentsFromBody(), "TestActionJ_2Param_id_value_extra_value");
				}
				using (HttpManager.OverrideContext("__test_action_j/child/id_value", HttpMethods.GET)) {
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					controllerManager.OnRequest();
					Assert.Equals(response.GetContentsFromBody(), "TestActionJ_Child_1Param_id_value");
				}
			});
		}

		public void RegisterController() {
			OnRequestTest(() => {
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
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
			});
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
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<ControllerManager>();
				Application.Ioc.RegisterMany<ControllerManager>();
				var controllerManager = Application.Ioc.Resolve<ControllerManager>();
				var actionGet = new Func<IActionResult>(() => { return new PlainResult("get"); });
				var actionPost = new Func<IActionResult>(() => { return new PlainResult("post"); });

				controllerManager.RegisterAction("__test_action/{id}", HttpMethods.GET, actionGet);
				controllerManager.RegisterAction("__test_action/{id}", HttpMethods.POST, actionPost);

				Assert.Equals(controllerManager.GetAction("__test_action/id", HttpMethods.GET), actionGet);
				Assert.Equals(controllerManager.GetAction("__test_action/id", HttpMethods.POST), actionPost);
				Assert.Equals(controllerManager.GetAction("__not_exist", HttpMethods.GET), null);
			}
		}

		public class TestController : IController {
			private static int _counter = 0;
			public int Value { get; } = Interlocked.Increment(ref _counter);

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

			[Action("__test_action_f", HttpMethods.POST)]
			public object TestActionF(IHttpPostedFile file) {
				return new { filename = file.FileName };
			}

			[TestActionFilter]
			[Action("__test_action_g", HttpMethods.GET)]
			public string TestActionG() {
				return "G";
			}

			[Action("__test_action_h", HttpMethods.GET)]
			public string TestActionH() {
				return "H";
			}

			[Action("__test_action_i", HttpMethods.GET)]
			public int TestActionI() {
				return Value;
			}

			[Action("__test_action_j/{id}", HttpMethods.GET)]
			public string TestActionJ_1Param(string id) {
				return $"TestActionJ_1Param_{id}";
			}

			[Action("__test_action_j/{id}/{extra}", HttpMethods.GET)]
			public string TestActionJ_2Param(string id, string extra) {
				return $"TestActionJ_2Param_{id}_{extra}";
			}

			[Action("__test_action_j/child/{id}", HttpMethods.GET)]
			public string TestActionJ_Child_1Param(string id) {
				return $"TestActionJ_Child_1Param_{id}";
			}
		}

		public class ActionParams {
			public string name { get; set; }
			public int age { get; set; }
		}

		public class TestActionFilter : ActionFilterAttribute {
			public override Func<IActionResult> Filter(Func<IActionResult> action) {
				return () => {
					var result = action();
					if (result is PlainResult)
						((PlainResult)result).Text += "Injected";
					return result;
				};
			}
		}
	}
}
