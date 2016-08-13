using System;
using ZKWeb.Cache;
using ZKWebStandard.Testing;
using ZKWebStandard.Utils;

namespace ZKWeb.Tests.Cache {
	[Tests]
	class IsolatedMemoryCacheTest {
		public void All() {
			var cache = new IsolatedMemoryCache<string, string>("Locale");
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
