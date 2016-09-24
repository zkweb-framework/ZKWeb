using System;
using System.Collections.Generic;
using System.Linq;
using ZKWebStandard.Collections;

namespace ZKWeb.Cache {
	/// <summary>
	/// Support isolate cache by given policies
	/// </summary>
	/// <typeparam name="TKey">Original key type</typeparam>
	/// <typeparam name="TValue">Value type</typeparam>
	public class IsolatedMemoryCache<TKey, TValue> :
		IKeyValueCache<TKey, TValue> {
		/// <summary>
		/// Cache isolation policies
		/// </summary>
		public IList<ICacheIsolationPolicy> IsolationPolicies { get; protected set; }
		/// <summary>
		/// Underlying cache
		/// </summary>
		public IKeyValueCache<IsolatedMemoryCacheKey<TKey>, TValue> UnderlyingCache { get; protected set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="isolationPolicyNames">Cache isolation policy names, resolve by IoC container</param>
		public IsolatedMemoryCache(params string[] isolationPolicyNames) :
			this((IEnumerable<string>)isolationPolicyNames) { }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="isolationPolicyNames">Cache isolation policy names, resolve by IoC container</param>
		public IsolatedMemoryCache(IEnumerable<string> isolationPolicyNames) :
			this(isolationPolicyNames.Select(name =>
				Application.Ioc.Resolve<ICacheIsolationPolicy>(serviceKey: name)).ToList()) { }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="isolationPolicies">Cache isolation policies</param>
		public IsolatedMemoryCache(params ICacheIsolationPolicy[] isolationPolicies) :
			this((IList<ICacheIsolationPolicy>)isolationPolicies) { }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="isolationPolicies">Cache isolation policies</param>
		public IsolatedMemoryCache(IList<ICacheIsolationPolicy> isolationPolicies) {
			IsolationPolicies = isolationPolicies;
			UnderlyingCache = new MemoryCache<IsolatedMemoryCacheKey<TKey>, TValue>();
		}

		/// <summary>
		/// Create cache key from original cache key and isolation keys
		/// </summary>
		/// <param name="key">Original cache key</param>
		/// <returns></returns>
		public IsolatedMemoryCacheKey<TKey> GenerateKey(TKey key) {
			var isolationKeys = new List<object>(IsolationPolicies.Count);
			foreach (var policy in IsolationPolicies) {
				isolationKeys.Add(policy.GetIsolationKey());
			}
			return new IsolatedMemoryCacheKey<TKey>(key, isolationKeys);
		}

		/// <summary>
		/// Put value to cache
		/// </summary>
		/// <param name="key">Original cache key</param>
		/// <param name="value">Cache value</param>
		/// <param name="keepTime">Keep time</param>
		public void Put(TKey key, TValue value, TimeSpan keepTime) {
			UnderlyingCache.Put(GenerateKey(key), value, keepTime);
		}

		/// <summary>
		/// Try to get cached value
		/// Return false if no exist value or exist value expired
		/// </summary>
		/// <param name="key">Original cache key</param>
		/// <param name="value">Cache value</param>
		/// <returns></returns>
		public bool TryGetValue(TKey key, out TValue value) {
			return UnderlyingCache.TryGetValue(GenerateKey(key), out value);
		}

		/// <summary>
		/// Remove cached value
		/// </summary>
		/// <param name="key">Original cache key</param>
		public void Remove(TKey key) {
			UnderlyingCache.Remove(GenerateKey(key));
		}

		/// <summary>
		/// Count cache
		/// </summary>
		/// <returns></returns>
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
