using ZKWebStandard.Collections;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Collections {
	[Tests]
	class SimpleDisposableTest {
		public void All() {
			// 手动多次执行
			int count = 0;
			var obj = new SimpleDisposable(() => count += 1);
			Assert.Equals(count, 0);
			obj.Dispose();
			Assert.Equals(count, 1);
			obj.Dispose();
			Assert.Equals(count, 1);
			// 使用using
			count = 0;
			using (new SimpleDisposable(() => count += 1)) {
			}
			Assert.Equals(count, 1);
		}
	}
}
