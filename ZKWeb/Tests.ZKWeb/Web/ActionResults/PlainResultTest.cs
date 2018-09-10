using System.IO;
using ZKWeb.Web.ActionResults;
using ZKWebStandard.Testing;
using ZKWebStandard.Web.Mock;

namespace ZKWeb.Tests.Web.ActionResults {
	[Tests]
	class PlainResultTest {
		public void WriteResponse() {
			var result = new PlainResult("test contents");
			var contextMock = new HttpContextMock();
			result.WriteResponse(contextMock.response);
			Assert.Equals(contextMock.response.StatusCode, 200);
			Assert.Equals(contextMock.response.ContentType, result.ContentType);
			Assert.Equals(contextMock.response.GetContentsFromBody(), "test contents");
		}
	}
}
