using System;
using System.Collections.Generic;
using ZKWebStandard.Collections;

namespace ZKWeb.Cache {
	/// <summary>
	/// Key-value cache supports isolate values by given policies<br/>
	/// 支持按策略隔离值的键值缓存类<br/>
	/// </summary>
	/// <typeparam name="TKey">Original key type</typeparam>
	/// <typeparam name="TValue">Value type</typeparam>
	/// <example>
	/// <code language="cs">
	/// var policies = new List&lt;ICacheIsolationPolicy&gt;();
	/// policies.Add(new CacheIsolateByDevice());
	/// policies.Add(new CacheIsolateByLocale());
	/// var underlyingCache = new MemoryCache&lt;IsolatedCacheKey$ltint&gt;, int&gt;();
	/// 
	/// var cache = new IsolatedKeyValueCache&lt;int, int&gt;(policies, underlyingCache);
	/// 
	/// // under mobile request
	/// { cache.Put(1, 100, TimeSpan.FromSeconds(30)); }
	/// 
	/// // under desktop request
	/// { cache.Put(1, 101, TimeSpan.FromSeconds(30)); }
	/// 
	/// // if request is from mobile, cached will be 100, otherwise it will be 101
	/// int cached;
	/// var success = cache.TryGetValue(1, out cached);
	/// </code>
	/// </example>
	/// <seealso cref="CacheFactory"/>
	/// <seealso cref="CacheFactoryOptions"/>
	public class IsolatedKeyValueCache<TKey, TValue> :
		IKeyValueCache<TKey, TValue> {
		/// <summary>
		/// The isolation policies of the cache<br/>
		/// 缓存隔离策略<br/>
		/// </summary>
		public IList<ICacheIsolationPolicy> IsolationPolicies { get; protected set; }
		/// <summary>
		/// Underlying cache instance<br/>
		/// 下层缓存实例<br/>
		/// </summary>
		public IKeyValueCache<IsolatedCacheKey<TKey>, TValue> UnderlyingCache { get; protected set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="isolationPolicies">The isolation policies of the cache</param>
		/// <param name="underlyingCache">Underlying cache instance</param>
		public IsolatedKeyValueCache(
			IList<ICacheIsolationPolicy> isolationPolicies,
			IKeyValueCache<IsolatedCacheKey<TKey>, TValue> underlyingCache) {
			IsolationPolicies = isolationPolicies;
			UnderlyingCache = underlyingCache;
		}

		/// <summary>
		/// Generate actual cache key from original cache key and isolation keys<br/>
		/// 根据原始缓存键和隔离键生成实际缓存键<br/>
		/// </summary>
		public IsolatedCacheKey<TKey> GenerateKey(TKey key) {
			var isolationKeys = new List<object>(IsolationPolicies.Count);
			foreach (var policy in IsolationPolicies) {
				isolationKeys.Add(policy.GetIsolationKey());
			}
			return new IsolatedCacheKey<TKey>(key, isolationKeys);
		}

		/// <summary>
		/// Put value to cache<br/>
		/// 插入值到缓存<br/>
		/// </summary>
		public void Put(TKey key, TValue value, TimeSpan keepTime) {
			UnderlyingCache.Put(GenerateKey(key), value, keepTime);
		}

		/// <summary>
		/// Try to get cached value<br/>
		/// Return false if there no value or it's expired<br/>
		/// 尝试获取缓存值，如果无值或者已过期则返回false<br/>
		/// </summary>
		public bool TryGetValue(TKey key, out TValue value) {
			return UnderlyingCache.TryGetValue(GenerateKey(key), out value);
		}

		/// <summary>
		/// Remove cached value<br/>
		/// 删除缓存值<br/>
		/// </summary>
		public void Remove(TKey key) {
			UnderlyingCache.Remove(GenerateKey(key));
		}

		/// <summary>
		/// Get how many values are cached<br/>
		/// 获取缓存了多少个值<br/>
		/// </summary>
		public int Count() {
			return UnderlyingCache.Count();
		}

		/// <summary>
		/// Remove all cache values<br/>
		/// 删除所有缓存值<br/>
		/// </summary>
		public void Clear() {
			UnderlyingCache.Clear();
		}
	}
}
