using System;
using System.IO;
using ZKWebStandard.Extensions;
using ZKWeb.Web.ActionResults;
using ZKWebStandard.Testing;
using ZKWebStandard.Web.Mock;
using ZKWeb.Storage;
using ZKWeb.Tests.Storage;

namespace ZKWeb.Tests.Web.ActionResults {
	[Tests]
	class FileResultTest {
		public void WriteResponse() {
			using (var layout = new TestDirectoryLayout()) {
				layout.WriteAppDataFile("static/__test.txt", "test contents");
				var pathManager = Application.Ioc.Resolve<LocalPathManager>();
				var resourcePath = pathManager.GetResourceFullPath("static/__test.txt");
				var lastModified = File.GetLastWriteTimeUtc(resourcePath).Truncate();
				Assert.Equals(File.ReadAllText(resourcePath), "test contents");

				var ifModifiedSinces = new DateTime?[] { null, DateTime.UtcNow.AddDays(1), lastModified };
				foreach (var ifModifiedSince in ifModifiedSinces) {
#pragma warning disable CS0618
					var result = new FileResult(resourcePath, ifModifiedSince);
#pragma warning restore CS0618
					var contextMock = new HttpContextMock();
					result.WriteResponse(contextMock.response);
					if (ifModifiedSince == lastModified) {
						Assert.Equals(contextMock.response.StatusCode, 304);
						Assert.Equals(contextMock.response.GetContentsFromBody(), "");
					} else {
						Assert.Equals(contextMock.response.StatusCode, 200);
						Assert.Equals(contextMock.response.ContentType, "text/plain");
						Assert.Equals(contextMock.response.GetContentsFromBody(), "test contents");
					}
					contextMock.request.headers["If-Modified-Since"] = (
						contextMock.response.headers["Last-Modified"][0]);
					Assert.Equals(contextMock.request.GetIfModifiedSince(), lastModified);
				}
			}
		}
	}
}
