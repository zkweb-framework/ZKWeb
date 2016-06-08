using Moq;
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
			var responseMock = new Mock<HttpResponseBase>();
			responseMock.Setup(r => r.Redirect(It.Is<string>(s => s == url))).Verifiable();
			result.WriteResponse(responseMock.Object);
			responseMock.Verify();

			result = new RedirectResult(url, true);
			responseMock = new Mock<HttpResponseBase>();
			responseMock.Setup(r => r.RedirectPermanent(It.Is<string>(s => s == url))).Verifiable();
			result.WriteResponse(responseMock.Object);
			responseMock.Verify();
		}
	}
}
