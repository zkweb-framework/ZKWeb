using System;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;
using ZKWebStandard.Web;

namespace ZKWebStandard.Tests.Collections {
	[Tests]
	class HttpContextCacheTest {
		public void All() {
			var httpContextCache = new HttpContextCache<int, string>();
			using (HttpManager.OverrideContext("", "GET")) {
				// Put
				httpContextCache.Put(1, "TestDataA", TimeSpan.FromMinutes(1));
				httpContextCache.Put(2, "TestDataB", TimeSpan.FromMinutes(2));
				httpContextCache.Put(3, "TestDataC", TimeSpan.FromMinutes(3));
				// GetOrDefault
				Assert.Equals(httpContextCache.GetOrDefault(1), "TestDataA");
				Assert.Equals(httpContextCache.GetOrDefault(2), "TestDataB");
				Assert.Equals(httpContextCache.GetOrDefault(3), "TestDataC");
				Assert.Equals(httpContextCache.GetOrDefault(100), null);
				Assert.Equals(httpContextCache.GetOrDefault(100, "Default"), "Default");
				// GetOrCreate
				Assert.Equals(httpContextCache.GetOrCreate(
					101, () => "create 101", TimeSpan.FromSeconds(100)), "create 101");
				Assert.Equals(httpContextCache.GetOrDefault(101), "create 101");
				Assert.Equals(httpContextCache.GetOrCreate(
					101, () => "create 101 again", TimeSpan.FromSeconds(100)), "create 101");
				Assert.Equals(httpContextCache.GetOrDefault(101), "create 101");
				httpContextCache.Remove(101);
				// Remove
				httpContextCache.Remove(2);
				httpContextCache.Remove(3);
				Assert.Equals(httpContextCache.Count(), 1);
				Assert.Equals(httpContextCache.GetOrDefault(1), "TestDataA");
				Assert.Equals(httpContextCache.GetOrDefault(2), null);
				Assert.Equals(httpContextCache.GetOrDefault(3), null);
			}
			// Destory value after context end
			using (HttpManager.OverrideContext("", "GET")) {
				httpContextCache.Put(1, "TestDataA", TimeSpan.FromMinutes(1));
				Assert.Equals(httpContextCache.GetOrDefault(1), "TestDataA");
			}
			using (HttpManager.OverrideContext("", "GET")) {
				Assert.Equals(httpContextCache.GetOrDefault(1), null);
			}
		}
	}
}
