using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

		public void GetReferer() {
			using (HttpManager.OverrideContext("", "POST")) {
				var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
				Assert.Equals(request.GetReferer(), null);
				request.headers["Referer"] = "http://abc.com/abc.html";
				Assert.Equals(request.GetReferer().ToString(), "http://abc.com/abc.html");
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
				var allParams = request.GetAll().ToDictionary(p => p.First, p => p.Second);
				Assert.Equals(allParams.GetOrDefault("a")[0], "1");
				Assert.Equals(allParams.GetOrDefault("b")[0], "2");
				Assert.Equals(allParams.GetOrDefault("c"), null);
			}
		}

		public void GetAllDictionary() {
			using (HttpManager.OverrideContext("/?a=1&b=2&a=3", "POST")) {
				var request = HttpManager.CurrentContext.Request;
				var allParams = request.GetAllDictionary();
				Assert.Equals(allParams.GetOrDefault("a")[0], "1");
				Assert.Equals(allParams.GetOrDefault("a")[1], "3");
				Assert.Equals(allParams.GetOrDefault("b")[0], "2");
				Assert.Equals(allParams.GetOrDefault("c"), null);
			}
		}

		public void GetAllAs() {
			// json
			using (HttpManager.OverrideContext("", "POST")) {
				var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
				request.contentType = "application/json";
				request.body = new MemoryStream(Encoding.UTF8.GetBytes("{ a: 'xxx', b: 1 }"));
				var result = request.GetAllAs<TestData>();
				Assert.Equals(result.a, "xxx");
				Assert.Equals(result.b, 1);
				Assert.Equals(result.c, null);
			}

			// query dictionary
			using (HttpManager.OverrideContext("/?a=xxx&b=1", "GET")) {
				var request = HttpManager.CurrentContext.Request;
				var result_a = request.GetAllAs<IDictionary<string, object>>();
				var result_b = request.GetAllAs<IDictionary<string, string>>();
				Assert.Equals(result_a.GetOrDefault("a"), "xxx");
				Assert.Equals(result_a.GetOrDefault<int>("b"), 1);
				Assert.Equals(result_b.GetOrDefault("a"), "xxx");
				Assert.Equals(result_b.GetOrDefault("b"), "1");
			}

			// query
			using (HttpManager.OverrideContext("/?a=xxx&b=1", "GET")) {
				var file = new HttpPostFileMock();
				var request = (HttpRequestMock)HttpManager.CurrentContext.Request;
				request.postedFiles["c"] = file;
				var result = request.GetAllAs<TestData>();
				Assert.Equals(result.a, "xxx");
				Assert.Equals(result.b, 1);
				Assert.Equals(result.c, file);
			}
		}

		public class TestData {
			public string a { get; set; }
			public int b { get; set; }
			public IHttpPostedFile c { get; set; }
		}
	}
}
