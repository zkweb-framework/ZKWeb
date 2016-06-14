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
			Assert.Equals(contextMock.response.StatusCode, 200);
			Assert.Equals(contextMock.response.ContentType, "text/plain");
			contextMock.response.body.Seek(0, SeekOrigin.Begin);
			Assert.Equals(new StreamReader(contextMock.response.body).ReadToEnd(), "test contents");
		}
	}
}
