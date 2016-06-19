using ZKWebStandard.Utils;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Functions {
	[Tests]
	class SystemUtilsTest {
		public void GetUsedMemoryBytes() {
			var value = SystemUtils.GetUsedMemoryBytes();
			Assert.IsTrueWith(value > 0, value);
		}
	}
}
