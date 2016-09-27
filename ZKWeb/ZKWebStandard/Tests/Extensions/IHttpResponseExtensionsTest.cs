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
			var contextMock = new HttpContextMock();
			contextMock.response.RedirectByScript("testurl");
			var contents = contextMock.response.GetContentsFromBody();
			Assert.IsTrueWith(contents.Contains("testurl"), contents);
			Assert.IsTrue(contextMock.response.isEnd);
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
				Assert.Equals(response.GetContentsFromBody(), "test contents");
			}
		}

#pragma warning disable CS0618
		public void WriteFile() {
			using (HttpManager.OverrideContext("", "POST")) {
				var path = Path.GetTempFileName();
				File.WriteAllText(path, "test file contents");
				try {
					var response = (HttpResponseMock)HttpManager.CurrentContext.Response;
					response.WriteFile(path);
					Assert.Equals(response.GetContentsFromBody(), "test file contents");
				} finally {
					File.Delete(path);
				}
			}
		}
#pragma warning restore CS0618
	}
}
