using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Utils.Collections;
using ZKWeb.Utils.UnitTest;
using ZKWeb.Web.ActionResults;

namespace ZKWeb.Tests.Web.ActionResults {
	[UnitTest]
	class JsonResultTest {
		public void WriteResponse() {
			var result = new JsonResult(new { a = 1 });
			var responseMock = new Mock<HttpResponseBase>();
			var exceptedResult = JsonConvert.SerializeObject(new { a = 1 });
			responseMock.SetupSet(r => r.ContentType = "application/json").Verifiable();
			responseMock.Setup(r => r.Write(It.Is<string>(s => s == exceptedResult))).Verifiable();
			result.WriteResponse(responseMock.Object);
			responseMock.Verify();

			result = new JsonResult(new { a = 1 }, Formatting.Indented);
			exceptedResult = JsonConvert.SerializeObject(new { a = 1 }, Formatting.Indented);
			responseMock.ResetCalls();
			result.WriteResponse(responseMock.Object);
			responseMock.Verify();
		}
	}
}
