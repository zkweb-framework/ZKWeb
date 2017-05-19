using System;
using System.Linq;
using ZKWebStandard.Collections;

namespace ZKWeb.Cache {
	/// <summary>
	/// Factory used to generate a key-value cache instance<br/>
	/// 用于生成一个键值缓存实例的工厂类<br/>
	/// </summary>
	/// <example>
	/// <code language="cs">
	/// var cacheFactory = Application.Ioc.Resolve&lt;ICacheFactory&gt;();
	/// 
	/// // create simple key value cache
	/// var simpleCache = cacheFactory.CreateCache&lt;int, int&gt;();
	/// 
	/// // create device isolated key value cache
	/// var options = CacheFactoryOptions.Default.WithIsolationPolicies("Device");
	/// var isolatedCache = cacheFactory.CreateCache&lt;int, int&gt;(options);
	/// </code>
	/// </example>
	/// <seealso cref="CacheFactoryOptions"/>
	internal class CacheFactory : ICacheFactory {
		/// <summary>
		/// Create cache instance<br/>
		/// 创建缓存实例<br/>
		/// </summary>
		/// <param name="options">Options used to generate the cache instance</param>
		/// <returns></returns>
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
