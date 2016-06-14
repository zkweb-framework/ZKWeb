using ZKWebStandard.Collections;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Collections {
	[Tests]
	class LazyCacheTest {
		public void All() {
			var createCount = 0;
			var cache = LazyCache.Create(() => { createCount += 1; return new object(); });

			Assert.IsTrue(!cache.IsValueCreated);
			var a = cache.Value;
			var b = cache.Value;
			Assert.IsTrue(cache.IsValueCreated);
			Assert.Equals(createCount, 1);
			Assert.IsTrue(object.ReferenceEquals(a, b));

			cache.Reset();
			Assert.IsTrue(!cache.IsValueCreated);
			a = cache.Value;
			b = cache.Value;
			Assert.IsTrue(cache.IsValueCreated);
			Assert.Equals(createCount, 2);
			Assert.IsTrue(object.ReferenceEquals(a, b));
		}
	}
}
