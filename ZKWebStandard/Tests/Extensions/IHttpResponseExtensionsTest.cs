using NSubstitute;
using System.IO;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;
using ZKWebStandard.Web;

namespace ZKWebStandard.Tests.Extensions {
	[Tests]
	class IHttpResponseExtensionsTest {
		public void RedirectByScript() {
			var responseMock = Substitute.For<IHttpResponse>();
			var stream = new MemoryStream();
			responseMock.Body.Returns(stream);
			responseMock.RedirectByScript("testurl");
			responseMock.Received().ContentType = "text/html";
			responseMock.Received().End();
			stream.Seek(0, SeekOrigin.Begin);
			using (var reader = new StreamReader(stream)) {
				var contents = reader.ReadToEnd();
				Assert.IsTrue(contents.Contains("testurl"));
			}
		}

		public void SetLastModified() {
			// TODO: test me
		}

		public void Write() {
			// TODO: test me
		}

		public void WriteFile() {
			// TODO: test me
		}
	}
}
