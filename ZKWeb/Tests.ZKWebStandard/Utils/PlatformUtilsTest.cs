using ZKWebStandard.Testing;
using ZKWebStandard.Utils;

namespace ZKWebStandard.Tests.Utils {
	[Tests]
	class PlatformUtilsTest {
		public void Check() {
			Assert.IsTrue(PlatformUtils.RunningOnUnix() != PlatformUtils.RunningOnWindows());
		}
	}
}
