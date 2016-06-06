using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Cache;
using ZKWeb.Utils.Functions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Tests.Cache {
	[UnitTest]
	class IsolatedMemoryCacheTest {
		public void All() {
			var cache = new IsolatedMemoryCache<string, string>("Locale");
			// 设置
			LocaleUtils.SetThreadLanguage("zh-CN");
			cache.Put("Key", "ValueForCN", TimeSpan.FromSeconds(5));
			LocaleUtils.SetThreadLanguage("en-US");
			cache.Put("Key", "ValueForUS", TimeSpan.FromSeconds(5));
			// 获取
			LocaleUtils.SetThreadLanguage("zh-CN");
			Assert.Equals(cache.GetOrDefault("Key"), "ValueForCN");
			LocaleUtils.SetThreadLanguage("en-US");
			Assert.Equals(cache.GetOrDefault("Key"), "ValueForUS");
			// 删除
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
