using Newtonsoft.Json;
using NSubstitute;
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
			var responseMock = Substitute.For<HttpResponseBase>();
			var exceptedResult = JsonConvert.SerializeObject(new { a = 1 });
			result.WriteResponse(responseMock);
			responseMock.Received().ContentType = "application/json";
			responseMock.Received().Write(exceptedResult);

			result = new JsonResult(new { a = 1 }, Formatting.Indented);
			exceptedResult = JsonConvert.SerializeObject(new { a = 1 }, Formatting.Indented);
			responseMock.ClearReceivedCalls();
			result.WriteResponse(responseMock);
			responseMock.Received().ContentType = "application/json";
			responseMock.Received().Write(exceptedResult);
		}
	}
}
