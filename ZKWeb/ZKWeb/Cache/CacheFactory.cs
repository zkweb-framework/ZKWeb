using System;
using System.Linq;
using ZKWebStandard.Collections;

namespace ZKWeb.Cache {
	/// <summary>
	/// Key-value cache factory
	/// </summary>
	internal class CacheFactory : ICacheFactory {
		/// <summary>
		/// Create cache by given options
		/// </summary>
		public virtual IKeyValueCache<TKey, TValue> CreateCache<TKey, TValue>(
			CacheFactoryOptions options = null) {
			options = options ?? CacheFactoryOptions.Default;
			if (options.Lifetime == CacheLifetime.Singleton) {
				if (options.IsolationPolicies.Any()) {
					// Singleton cache with isolation policies
					return new IsolatedKeyValueCache<TKey, TValue>(
						options.IsolationPolicies, new MemoryCache<IsolatedCacheKey<TKey>, TValue>());
				} else {
					// Singleton cache
					return new MemoryCache<TKey, TValue>();
				}
			} else if (options.Lifetime == CacheLifetime.PerHttpContext) {
				if (options.IsolationPolicies.Any()) {
					// Options error
					throw new ArgumentException("PerHttpContext shouldn't use with isolation policies");
				} else {
					// Per http context cache
					return new HttpContextCache<TKey, TValue>();
				}
			} else {
				throw new NotSupportedException($"Unsupported cache lifetime: {options.Lifetime}");
			}
		}
	}
}
