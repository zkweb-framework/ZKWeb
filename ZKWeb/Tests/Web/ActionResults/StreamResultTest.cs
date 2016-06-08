using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using ZKWeb.Utils.Collections;
using ZKWeb.Utils.UnitTest;
using ZKWeb.Web.ActionResults;

namespace ZKWeb.Tests.Web.ActionResults {
	[UnitTest]
	class StreamResultTest {
		public void WriteResponse() {
			var stream = new MemoryStream(Encoding.UTF8.GetBytes("test contents"));
			using (var result = new StreamResult(stream)) {
				var responseMock = new HttpResponseMock();
				responseMock.outputStream = new MemoryStream();
				result.WriteResponse(responseMock);
				responseMock.outputStream.Seek(0, SeekOrigin.Begin);
				Assert.Equals(responseMock.ContentType, "application/octet-stream");
				Assert.Equals(new StreamReader(responseMock.outputStream).ReadToEnd(), "test contents");
			}
		}
	}
}
