using NSubstitute;
using ZKWeb.Web.ActionResults;
using ZKWebStandard.Testing;
using ZKWebStandard.Web;

namespace ZKWeb.Tests.Web.ActionResults {
	[Tests]
	class RedirectResultTest {
		public void WriteResponse() {
			var url = "test_url";
			var result = new RedirectResult(url);
			var responseMock = Substitute.For<IHttpResponse>();
			result.WriteResponse(responseMock);
			responseMock.Received().Redirect(url, false);

			result = new RedirectResult(url, true);
			responseMock.ClearReceivedCalls();
			result.WriteResponse(responseMock);
			responseMock.Received().Redirect(url, true);
		}
	}
}
