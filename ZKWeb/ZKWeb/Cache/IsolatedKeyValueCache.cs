using System;
using System.Collections.Generic;
using ZKWebStandard.Collections;

namespace ZKWeb.Cache {
	/// <summary>
	/// Support isolate cache by given policies
	/// </summary>
	/// <typeparam name="TKey">Original key type</typeparam>
	/// <typeparam name="TValue">Value type</typeparam>
	public class IsolatedKeyValueCache<TKey, TValue> :
		IKeyValueCache<TKey, TValue> {
		/// <summary>
		/// Cache isolation policies
		/// </summary>
		public IList<ICacheIsolationPolicy> IsolationPolicies { get; protected set; }
		/// <summary>
		/// Underlying cache
		/// </summary>
		public IKeyValueCache<IsolatedCacheKey<TKey>, TValue> UnderlyingCache { get; protected set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="isolationPolicies">Cache isolation policies</param>
		/// <param name="underlyingCache">Underlying cache</param>
		public IsolatedKeyValueCache(
			IList<ICacheIsolationPolicy> isolationPolicies,
			IKeyValueCache<IsolatedCacheKey<TKey>, TValue> underlyingCache) {
			IsolationPolicies = isolationPolicies;
			UnderlyingCache = underlyingCache;
		}

		/// <summary>
		/// Create cache key from original cache key and isolation keys
		/// </summary>
		public IsolatedCacheKey<TKey> GenerateKey(TKey key) {
			var isolationKeys = new List<object>(IsolationPolicies.Count);
			foreach (var policy in IsolationPolicies) {
				isolationKeys.Add(policy.GetIsolationKey());
			}
			return new IsolatedCacheKey<TKey>(key, isolationKeys);
		}

		/// <summary>
		/// Put value to cache
		/// </summary>
		public void Put(TKey key, TValue value, TimeSpan keepTime) {
			UnderlyingCache.Put(GenerateKey(key), value, keepTime);
		}

		/// <summary>
		/// Try to get cached value
		/// Return false if no exist value or exist value expired
		/// </summary>
		public bool TryGetValue(TKey key, out TValue value) {
			return UnderlyingCache.TryGetValue(GenerateKey(key), out value);
		}

		/// <summary>
		/// Remove cached value
		/// </summary>
		public void Remove(TKey key) {
			UnderlyingCache.Remove(GenerateKey(key));
		}

		/// <summary>
		/// Count cache
		/// </summary>
		public int Count() {
			return UnderlyingCache.Count();
		}

		/// <summary>
		/// Clear cache
		/// </summary>
		public void Clear() {
			UnderlyingCache.Clear();
		}
	}
}
