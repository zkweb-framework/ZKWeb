using ZKWeb.Web.ActionResults;
using ZKWebStandard.Testing;
using ZKWebStandard.Web.Mock;

namespace ZKWeb.Tests.Web.ActionResults {
	[Tests]
	class RedirectResultTest {
		public void WriteResponse() {
			var url = "test_url";
			var result = new RedirectResult(url);
			var responseMock = new HttpResponseMock(null);
			result.WriteResponse(responseMock);
			Assert.Equals(responseMock.lastRedirect, url);
			Assert.IsTrue(!responseMock.lastRedirectIsPermanent);

			result = new RedirectResult(url, true);
			responseMock = new HttpResponseMock(null);
			result.WriteResponse(responseMock);
			Assert.Equals(responseMock.lastRedirect, url);
			Assert.IsTrue(responseMock.lastRedirectIsPermanent);
		}
	}
}
