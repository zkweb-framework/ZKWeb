using System;
using ZKWebStandard.Extensions;
using ZKWeb.Web.ActionResults;
using ZKWebStandard.Testing;
using ZKWebStandard.Web.Mock;
using ZKWeb.Storage;
using ZKWeb.Tests.Storage;

namespace ZKWeb.Tests.Web.ActionResults {
	[Tests]
	class FileEntryResultTest {
		public void WriteResponse() {
			using (var layout = new TestDirectoryLayout()) {
				layout.WriteAppDataFile("static/__test.txt", "test contents");
				var fileStorage = Application.Ioc.Resolve<IFileStorage>();
				var fileEntry = fileStorage.GetResourceFile("static/__test.txt");
				var lastModified = fileEntry.LastWriteTimeUtc.Truncate();
				Assert.Equals(fileEntry.ReadAllText(), "test contents");

				var ifModifiedSinces = new DateTime?[] { null, DateTime.UtcNow.AddDays(1), lastModified };
				foreach (var ifModifiedSince in ifModifiedSinces) {
					var result = new FileEntryResult(fileEntry, ifModifiedSince);
					var contextMock = new HttpContextMock();
					result.WriteResponse(contextMock.response);
					if (ifModifiedSince == lastModified) {
						// 304
						Assert.Equals(contextMock.response.StatusCode, 304);
						Assert.Equals(contextMock.response.GetContentsFromBody(), "");
					} else {
						// 200
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
