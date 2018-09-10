using System.IO;
using System.Text;
using ZKWeb.Web.ActionResults;
using ZKWebStandard.Collections;
using ZKWebStandard.Testing;
using ZKWebStandard.Web.Mock;

namespace ZKWeb.Tests.Web.ActionResults {
	[Tests]
	class StreamResultTest {
		public void WriteResponse() {
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("test contents"));
			using (var result = new StreamResult(stream)) {
				var contextMock = new HttpContextMock();
				result.WriteResponse(contextMock.response);
				Assert.Equals(contextMock.response.StatusCode, 200);
				Assert.Equals(contextMock.response.ContentType, "application/octet-stream");
				Assert.Equals(contextMock.response.GetContentsFromBody(), "test contents");
			}
		}

		public void WriteResponseWithRange() {
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("test contents"));
			using (var result = new StreamResult(stream, Pair.Create<long?, long?>(5, 11))) {
				var contextMock = new HttpContextMock();
				result.WriteResponse(contextMock.response);
				Assert.Equals(contextMock.response.StatusCode, 206);
				Assert.Equals(contextMock.response.ContentType, "application/octet-stream");
				Assert.Equals(contextMock.response.headers["Accept-Ranges"][0], "bytes");
				Assert.Equals(contextMock.response.headers["Content-Range"][0], "5-11/13");
				Assert.Equals(contextMock.response.GetContentsFromBody(), "content");
			}

			stream = new MemoryStream(Encoding.UTF8.GetBytes("test contents"));
			using (var result = new StreamResult(stream, Pair.Create<long?, long?>(5, null))) {
				var contextMock = new HttpContextMock();
				result.WriteResponse(contextMock.response);
				Assert.Equals(contextMock.response.StatusCode, 206);
				Assert.Equals(contextMock.response.ContentType, "application/octet-stream");
				Assert.Equals(contextMock.response.headers["Accept-Ranges"][0], "bytes");
				Assert.Equals(contextMock.response.headers["Content-Range"][0], "5-12/13");
				Assert.Equals(contextMock.response.GetContentsFromBody(), "contents");
			}
		}
	}
}
