using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Utils.UnitTest;
using ZKWeb.Web.ActionResults;

namespace ZKWeb.Tests.Web.ActionResults {
	[UnitTest]
	class RedirectResultTest {
		public void WriteResponse() {
			var url = "test_url";
			var result = new RedirectResult(url);
			var responseMock = Substitute.For<HttpResponseBase>();
			result.WriteResponse(responseMock);
			responseMock.Received().Redirect(url);

			result = new RedirectResult(url, true);
			responseMock.ClearReceivedCalls();
			result.WriteResponse(responseMock);
			responseMock.Received().RedirectPermanent(url);
		}
	}
}
