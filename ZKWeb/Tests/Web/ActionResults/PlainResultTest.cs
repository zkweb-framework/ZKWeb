using Moq;
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
			var responseMock = new Mock<HttpResponseBase>();
			var exceptedResult = "test contents";
			responseMock.SetupSet(r => r.ContentType = "text/plain").Verifiable();
			responseMock.Setup(r => r.Write(It.Is<string>(s => s == exceptedResult))).Verifiable();
			result.WriteResponse(responseMock.Object);
			responseMock.Verify();
		}
	}
}
