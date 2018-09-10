using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Extensions {
	[Tests]
	class DecimalExtensionsTest {
		public void Normalize() {
			Assert.Equals((12.3000M).Normalize().ToString(), "12.3");
			Assert.Equals((0.0001M).Normalize().ToString(), "0.0001");
			Assert.Equals((0.000001000M).Normalize().ToString(), "0.000001");
		}
	}
}
