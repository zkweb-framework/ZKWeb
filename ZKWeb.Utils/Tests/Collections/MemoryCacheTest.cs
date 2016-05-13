using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Collections;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Collections {
	[UnitTest]
	class MemoryCacheTest {
		class TestCache : MemoryCache<int, string> {
			public new IDictionary<int, Tuple<string, DateTime>> Cache { get { return base.Cache; } }
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
			Assert.Equals(memoryCache.Cache[1].Item1, "TestDataA");
			Assert.IsTrue(memoryCache.Cache[1].Item2 >= now.AddMinutes(1));
			Assert.Equals(memoryCache.Cache[2].Item1, "TestDataB");
			Assert.IsTrue(memoryCache.Cache[2].Item2 >= now.AddMinutes(2));
			Assert.Equals(memoryCache.Cache[3].Item1, "TestDataC");
			Assert.IsTrue(memoryCache.Cache[3].Item2 >= now.AddMinutes(3));
			// GetOrDefault
			Assert.Equals(memoryCache.GetOrDefault(1), "TestDataA");
			Assert.Equals(memoryCache.GetOrDefault(2), "TestDataB");
			Assert.Equals(memoryCache.GetOrDefault(3), "TestDataC");
			Assert.Equals(memoryCache.GetOrDefault(100), null);
			Assert.Equals(memoryCache.GetOrDefault(100, "Default"), "Default");
			// GetOrDefault，已过期未删除时
			memoryCache.Put(1, "TestDataA", TimeSpan.FromMinutes(-1));
			Assert.Equals(memoryCache.GetOrDefault(1), null);
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
