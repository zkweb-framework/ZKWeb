using System;
using ZKWeb.Cache;
using ZKWeb.Cache.Policies;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;
using ZKWebStandard.Utils;

namespace ZKWeb.Tests.Cache {
	[Tests]
	class IsolatedKeyValueCacheTest {
		public void All() {
			var cache = new IsolatedKeyValueCache<string, string>(
				new[] { new CacheIsolateByLocale() },
				new MemoryCache<IsolatedCacheKey<string>, string>());
			// Set
			LocaleUtils.SetThreadLanguage("zh-CN");
			cache.Put("Key", "ValueForCN", TimeSpan.FromSeconds(5));
			LocaleUtils.SetThreadLanguage("en-US");
			cache.Put("Key", "ValueForUS", TimeSpan.FromSeconds(5));
			// Get
			LocaleUtils.SetThreadLanguage("zh-CN");
			Assert.Equals(cache.GetOrDefault("Key"), "ValueForCN");
			LocaleUtils.SetThreadLanguage("en-US");
			Assert.Equals(cache.GetOrDefault("Key"), "ValueForUS");
			// Remove
			LocaleUtils.SetThreadLanguage("zh-CN");
			cache.Remove("Key");
			Assert.Equals(cache.GetOrDefault("Key"), null);
			LocaleUtils.SetThreadLanguage("en-US");
			Assert.Equals(cache.GetOrDefault("Key"), "ValueForUS");
			cache.Remove("Key");
			Assert.Equals(cache.GetOrDefault("Key"), null);
		}
	}
}
