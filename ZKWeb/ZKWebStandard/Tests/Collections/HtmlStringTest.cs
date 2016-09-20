using ZKWebStandard.Collection;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Collections {
	[Tests]
	class HtmlStringTest {
		public void Encode() {
			Assert.Equals(HtmlString.Encode("asd'\"<>").ToString(), "asd&#39;&quot;&lt;&gt;");
		}

		public void Decode() {
			Assert.Equals(HtmlString.Encode("asd'\"<>").Decode(), "asd'\"<>");
		}
	}
}
