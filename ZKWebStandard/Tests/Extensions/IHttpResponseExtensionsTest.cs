using NSubstitute;
using System;
using System.IO;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;
using ZKWebStandard.Web;
using ZKWebStandard.Web.Mock;

namespace ZKWebStandard.Tests.Extensions {
	[Tests]
	class IHttpResponseExtensionsTest {
		public void RedirectByScript() {
			var responseMock = Substitute.For<IHttpResponse>();
			var stream = new MemoryStream();
			responseMock.Body.Returns(stream);
			responseMock.RedirectByScript("testurl");
			responseMock.Received().ContentType = "text/html";
			responseMock.Received().End();
			stream.Seek(0, SeekOrigin.Begin);
			using (var reader = new StreamReader(stream)) {
				var contents = reader.ReadToEnd();
				Assert.IsTrue(contents.Contains("testurl"));
			}
		}

		public void SetLastModified() {
			using (HttpManager.OverrideContext("", "POST")) {
				var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
				response.SetLastModified(new DateTime(2016, 06, 13, 03, 09, 22, DateTimeKind.Utc));
				Assert.Equals(response.headers["Last-Modified"][0], "Mon, 13 Jun 2016 03:09:22 GMT");
			}
		}

		public void Write() {
			using (HttpManager.OverrideContext("", "POST")) {
				var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
				response.Write("test contents");
				response.body.Seek(0, SeekOrigin.Begin);
				Assert.Equals(new StreamReader(response.body).ReadToEnd(), "test contents");
			}
		}

		public void WriteFile() {
			using (HttpManager.OverrideContext("", "POST")) {
				var path = Path.GetTempFileName();
				File.WriteAllText(path, "test file contents");
				try {
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					response.WriteFile(path);
					response.body.Seek(0, SeekOrigin.Begin);
					Assert.Equals(new StreamReader(response.body).ReadToEnd(), "test file contents");
				} finally {
					File.Delete(path);
				}
			}
		}
	}
}
