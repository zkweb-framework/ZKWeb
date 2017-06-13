using ZKWebStandard.Collection;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Collections {
	[Tests]
	class HtmlStringTest {
		public void Encode() {
			Assert.Equals(HtmlString.Encode("asd'\"<>").ToString(), "asd&#39;&quot;&lt;&gt;");
			Assert.Equals(1, 0);
		}

		public void Decode() {
			Assert.Equals(HtmlString.Encode("asd'\"<>").Decode(), "asd'\"<>");
		}
	}
}
