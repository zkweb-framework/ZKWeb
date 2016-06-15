using System;
using System.Collections.Generic;
using System.Linq;
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
			using (HttpManager.OverrideContext("", "POST")) {
				var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
				request.headers["User-Agent"] = "my custom user agent";
				Assert.Equals(request.GetUserAgent(), "my custom user agent");
			}
		}

		public void GetAcceptLanguages() {
			using (HttpManager.OverrideContext("", "POST")) {
				var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
				request.headers["Accept-Language"] = "en-US,en;q=0.7,zh-CN;q=0.3";
				var languages = request.GetAcceptLanguages();
				Assert.IsTrueWith(languages.SequenceEqual(new[] { "en-US", "en", "zh-CN" }), languages);
			}
		}

		public void GetIfModifiedSince() {
			using (HttpManager.OverrideContext("", "POST")) {
				var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
				request.headers["If-Modified-Since"] = "Mon, 13 Jun 2016 03:09:22 GMT";
				Assert.Equals(
					request.GetIfModifiedSince(),
					new DateTime(2016, 06, 13, 03, 09, 22, DateTimeKind.Utc));
			}
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
