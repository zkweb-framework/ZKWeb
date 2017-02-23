using Newtonsoft.Json;
using System.IO;
using ZKWeb.Web.ActionResults;
using ZKWebStandard.Testing;
using ZKWebStandard.Web.Mock;

namespace ZKWeb.Tests.Web.ActionResults {
	[Tests]
	class JsonResultTest {
		public void WriteResponse() {
			var formats = new[] { Formatting.None, Formatting.Indented };
			foreach (var format in formats) {
				var result = new JsonResult(new { a = 1 }, format);
				var contextMock = new HttpContextMock();
				var exceptedResult = JsonConvert.SerializeObject(new { a = 1 }, format);
				result.WriteResponse(contextMock.response);
				Assert.Equals(contextMock.response.StatusCode, 200);
				Assert.Equals(contextMock.response.ContentType, result.ContentType);
				Assert.Equals(contextMock.response.GetContentsFromBody(), exceptedResult);
			}
		}
	}
}
