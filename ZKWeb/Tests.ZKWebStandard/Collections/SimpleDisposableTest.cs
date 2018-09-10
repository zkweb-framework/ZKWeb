using ZKWebStandard.Collections;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Collections {
	[Tests]
	class SimpleDisposableTest {
		public void All() {
			// Dispose many times
			int count = 0;
			var obj = new SimpleDisposable(() => count += 1);
			Assert.Equals(count, 0);
			obj.Dispose();
			Assert.Equals(count, 1);
#pragma warning disable S3966 // Objects should not be disposed more than once
			obj.Dispose();
#pragma warning restore S3966 // Objects should not be disposed more than once
			Assert.Equals(count, 1);
			// With using directive
			count = 0;
			using (new SimpleDisposable(() => count += 1)) {
				// Do nothing
			}
			Assert.Equals(count, 1);
		}
	}
}
