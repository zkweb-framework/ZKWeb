using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Utils.Collections;
using ZKWeb.Utils.UnitTest;
using ZKWeb.Web.ActionResults;

namespace ZKWeb.Tests.Web.ActionResults {
	[UnitTest]
	class ImageResultTest {
		public void WriteResponse() {
			var image = new Bitmap(123, 456);
			var result = new ImageResult(image);
			var responseMock = new HttpResponseMock();
			responseMock.outputStream = new MemoryStream();
			result.WriteResponse(responseMock);
			responseMock.outputStream.Seek(0, SeekOrigin.Begin);
			var imageVerify = Image.FromStream(responseMock.outputStream);
			Assert.Equals(imageVerify.Width, 123);
			Assert.Equals(imageVerify.Height, 456);
		}
	}
}
