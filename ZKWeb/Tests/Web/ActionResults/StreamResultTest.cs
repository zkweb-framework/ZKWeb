using System.IO;
using System.Text;
using ZKWeb.Web.ActionResults;
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
	}
}
