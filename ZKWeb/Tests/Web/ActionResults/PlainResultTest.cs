using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Utils.UnitTest;
using ZKWeb.Web.ActionResults;

namespace ZKWeb.Tests.Web.ActionResults {
	[UnitTest]
	class PlainResultTest {
		public void WriteResponse() {
			var result = new PlainResult("test contents");
			var responseMock = Substitute.For<HttpResponseBase>();
			result.WriteResponse(responseMock);
			responseMock.Received().ContentType = "text/plain";
			responseMock.Received().Write("test contents");
		}
	}
}
