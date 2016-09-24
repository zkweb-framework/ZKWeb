using System;

namespace ZKWebStandard.Collections {
	/// <summary>
	/// Interface of key-value cache
	/// It should be thread safe
	/// </summary>
	public interface IKeyValueCache<TKey, TValue> {
		/// <summary>
		/// Put value to cache
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="value">Cache value</param>
		/// <param name="keepTime">Keep time</param>
		void Put(TKey key, TValue value, TimeSpan keepTime);

		/// <summary>
		/// Try to get cached value
		/// Return false if no exist value or exist value expired
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="value">Cache value</param>
		/// <returns></returns>
		bool TryGetValue(TKey key, out TValue value);

		/// <summary>
		/// Remove cached value
		/// </summary>
		/// <param name="key">Cache key</param>
		void Remove(TKey key);

		/// <summary>
		/// Count all cached values
		/// </summary>
		/// <returns></returns>
		int Count();

		/// <summary>
		/// Clear all cached values
		/// </summary>
		void Clear();
	}
}
