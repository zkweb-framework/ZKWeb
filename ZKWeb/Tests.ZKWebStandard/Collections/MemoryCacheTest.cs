using System;
using System.Collections.Generic;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Collections {
	[Tests]
	class MemoryCacheTest {
		class TestCache : MemoryCache<int, string> {
			public new IDictionary<int, Pair<string, DateTime>> Cache { get { return base.Cache; } }
			public new DateTime LastRevokeExpires {
				get { return base.LastRevokeExpires; }
				set { base.LastRevokeExpires = value; }
			}
		}

		public void All() {
			var memoryCache = new TestCache();
			// Put
			var now = DateTime.UtcNow;
			memoryCache.Put(1, "TestDataA", TimeSpan.FromMinutes(1));
			memoryCache.Put(2, "TestDataB", TimeSpan.FromMinutes(2));
			memoryCache.Put(3, "TestDataC", TimeSpan.FromMinutes(3));
			Assert.Equals(memoryCache.Count(), 3);
			Assert.Equals(memoryCache.Cache[1].First, "TestDataA");
			Assert.IsTrue(memoryCache.Cache[1].Second >= now.AddMinutes(1));
			Assert.Equals(memoryCache.Cache[2].First, "TestDataB");
			Assert.IsTrue(memoryCache.Cache[2].Second >= now.AddMinutes(2));
			Assert.Equals(memoryCache.Cache[3].First, "TestDataC");
			Assert.IsTrue(memoryCache.Cache[3].Second >= now.AddMinutes(3));
			// GetOrDefault
			Assert.Equals(memoryCache.GetOrDefault(1), "TestDataA");
			Assert.Equals(memoryCache.GetOrDefault(2), "TestDataB");
			Assert.Equals(memoryCache.GetOrDefault(3), "TestDataC");
			Assert.Equals(memoryCache.GetOrDefault(100), null);
			Assert.Equals(memoryCache.GetOrDefault(100, "Default"), "Default");
			// GetOrDefault, expired
			memoryCache.Put(1, "TestDataA", TimeSpan.FromMinutes(-1));
			Assert.Equals(memoryCache.GetOrDefault(1), null);
			// GetOrCreate
			Assert.Equals(memoryCache.GetOrCreate(
				101, () => "create 101", TimeSpan.FromSeconds(100)), "create 101");
			Assert.Equals(memoryCache.GetOrDefault(101), "create 101");
			Assert.Equals(memoryCache.GetOrCreate(
				101, () => "create 101 again", TimeSpan.FromSeconds(100)), "create 101");
			Assert.Equals(memoryCache.GetOrDefault(101), "create 101");
			memoryCache.Remove(101);
			// RevokeExpires
			memoryCache.LastRevokeExpires = DateTime.UtcNow.AddSeconds(-181);
			memoryCache.GetOrDefault(0);
			Assert.Equals(memoryCache.Count(), 2);
			Assert.Equals(memoryCache.GetOrDefault(1), null);
			Assert.Equals(memoryCache.GetOrDefault(2), "TestDataB");
			Assert.Equals(memoryCache.GetOrDefault(3), "TestDataC");
			// Remove
			memoryCache.Remove(2);
			memoryCache.Remove(3);
			Assert.Equals(memoryCache.Count(), 0);
			Assert.Equals(memoryCache.GetOrDefault(1), null);
			Assert.Equals(memoryCache.GetOrDefault(2), null);
			Assert.Equals(memoryCache.GetOrDefault(3), null);
		}
	}
}
