using System.Collections.Generic;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;
using ZKWebStandard.Web;
using ZKWebStandard.Web.Mock;

namespace ZKWebStandard.Tests.Extensions {
	[Tests]
	class IHttpRequestExtensionsTest {
		public void IsAjaxRequest() {
			using (HttpManager.OverrideContext("", "POST")) {
				var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
				Assert.IsTrue(!request.IsAjaxRequest());
				request.headers["X-Requested-With"] = "XMLHttpRequest";
				Assert.IsTrue(request.IsAjaxRequest());
			}
		}

		public void GetUserAgent() {
			// TODO: test me
		}

		public void GetAcceptLanguages() {
			// TODO: test me
		}

		public void GetLastModified() {
			// TODO: test me
		}

		public void Get() {
			using (HttpManager.OverrideContext("/?a=1&b=2", "POST")) {
				var request = HttpManager.CurrentContext.Request;
				Assert.Equals(request.Get<string>("a"), "1");
				Assert.Equals(request.Get<int>("b"), 2);
				Assert.Equals(request.Get<object>("c"), null);
			}
		}

		public void GetAll() {
			using (HttpManager.OverrideContext("/?a=1&b=2", "POST")) {
				var request = HttpManager.CurrentContext.Request;
				var allParams = new Dictionary<string, string>();
				request.GetAll().ForEach(pair => allParams[pair.First] = pair.Second);
				Assert.Equals(allParams.GetOrDefault("a"), "1");
				Assert.Equals(allParams.GetOrDefault("b"), "2");
				Assert.Equals(allParams.GetOrDefault("c"), null);
			}
		}
	}
}
