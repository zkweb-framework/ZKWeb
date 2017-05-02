using System.DrawingCore;
using System.IO;
using ZKWeb.Web.ActionResults;
using ZKWebStandard.Testing;
using ZKWebStandard.Web.Mock;

namespace ZKWeb.Tests.Web.ActionResults {
	[Tests]
	class ImageResultTest {
		public void WriteResponse() {
			var image = new Bitmap(123, 456);
			var result = new ImageResult(image);
			var contextMock = new HttpContextMock();
			result.WriteResponse(contextMock.response);
			contextMock.response.body.Seek(0, SeekOrigin.Begin);
			Assert.Equals(contextMock.response.StatusCode, 200);
			Assert.Equals(contextMock.response.ContentType, "image/jpeg");
			var imageVerify = Image.FromStream(contextMock.response.body);
			Assert.Equals(imageVerify.Width, 123);
			Assert.Equals(imageVerify.Height, 456);
		}
	}
}
