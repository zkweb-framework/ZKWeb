using Newtonsoft.Json;
using System.IO;
using ZKWeb.Web.ActionResults;
using ZKWebStandard.Testing;
using ZKWebStandard.Web.Mock;

namespace ZKWeb.Tests.Web.ActionResults {
	[Tests]
	class JsonResultTest {
		public void WriteResponse() {
			var result = new JsonResult(new { a = 1 });
			var contextMock = new HttpContextMock();
			var exceptedResult = JsonConvert.SerializeObject(new { a = 1 });
			Assert.Equals(contextMock.response.StatusCode, 200);
			Assert.Equals(contextMock.response.ContentType, "application/json");
			contextMock.response.body.Seek(0, SeekOrigin.Begin);
			Assert.Equals(new StreamReader(contextMock.response.body).ReadToEnd(), exceptedResult);

			result = new JsonResult(new { a = 1 }, Formatting.Indented);
			exceptedResult = JsonConvert.SerializeObject(new { a = 1 }, Formatting.Indented);
			Assert.Equals(contextMock.response.StatusCode, 200);
			Assert.Equals(contextMock.response.ContentType, "application/json");
			contextMock.response.body.Seek(0, SeekOrigin.Begin);
			Assert.Equals(new StreamReader(contextMock.response.body).ReadToEnd(), exceptedResult);
		}
	}
}
