using ZKWebStandard.Testing;
using ZKWebStandard.Utils;

namespace ZKWebStandard.Tests.Utils {
	[Tests]
	class MimeUtilsTest {
		public void GetMimeType() {
			Assert.Equals(MimeUtils.GetMimeType("1.html"), "text/html");
			Assert.Equals(MimeUtils.GetMimeType("1.vbs"), "text/vbscript");
		}
	}
}
