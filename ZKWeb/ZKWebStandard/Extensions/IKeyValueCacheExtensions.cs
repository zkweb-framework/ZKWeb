using System;
using ZKWebStandard.Collections;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// IKeyValueCache extension methods<br/>
	/// <br/>
	/// </summary>
	public static class IKeyValueCacheExtensions {
		/// <summary>
		/// Get cached value<br/>
		/// Return default value if no exist value or exist value expired<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="cache">Cache</param>
		/// <param name="key">Key</param>
		/// <param name="defaultValue">The default value</param>
		/// <returns></returns>
		public static TValue GetOrDefault<TKey, TValue>(
			this IKeyValueCache<TKey, TValue> cache,
			TKey key, TValue defaultValue = default(TValue)) {
			TValue value;
			if (cache.TryGetValue(key, out value)) {
				return value;
			}
			return defaultValue;
		}

		/// <summary>
		/// Get cached value<br/>
		/// Generate a new value and store it to cache if the no exist value or exist value expired<br/>
		/// Attention: This is not an atomic operation<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="cache">Cache</param>
		/// <param name="key">Key</param>
		/// <param name="creator">Function to create default value</param>
		/// <param name="keepTime">Keep time</param>
		/// <returns></returns>
		public static TValue GetOrCreate<TKey, TValue>(
			this IKeyValueCache<TKey, TValue> cache,
			TKey key, Func<TValue> creator, TimeSpan keepTime) {
			TValue value;
			if (keepTime == TimeSpan.Zero || !cache.TryGetValue(key, out value)) {
				value = creator();
				cache.Put(key, value, keepTime);
			}
			return value;
		}
	}
}
