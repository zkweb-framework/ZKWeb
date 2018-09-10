using System.IO;
using System.Text;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Extensions {
	[Tests]
	class StreamExtensionTest {
		public void CopyTo() {
			using (var source = new MemoryStream(Encoding.UTF8.GetBytes("0123456789")))
			using (var desitination = new MemoryStream()) {
				source.CopyTo(desitination, 1024, 1, 3);
				desitination.Seek(0, SeekOrigin.Begin);
				Assert.Equals(new StreamReader(desitination).ReadToEnd(), "123");

				desitination.Seek(0, SeekOrigin.Begin);
				source.CopyTo(desitination, 1024, 7, 5);
				desitination.Seek(0, SeekOrigin.Begin);
				Assert.Equals(new StreamReader(desitination).ReadToEnd(), "789");
			}
		}
	}
}
