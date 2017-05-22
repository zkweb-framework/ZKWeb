using System;

namespace ZKWebStandard.Collections {
	/// <summary>
	/// Interface of key-value cache<br/>
	/// It should be thread safe<br/>
	/// <br/>
	/// <br/>
	/// </summary>
	public interface IKeyValueCache<TKey, TValue> {
		/// <summary>
		/// Put value to cache<br/>
		/// <br/>
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="value">Cache value</param>
		/// <param name="keepTime">Keep time</param>
		void Put(TKey key, TValue value, TimeSpan keepTime);

		/// <summary>
		/// Try to get cached value<br/>
		/// Return false if no exist value or exist value expired<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="value">Cache value</param>
		/// <returns></returns>
		bool TryGetValue(TKey key, out TValue value);

		/// <summary>
		/// Remove cached value<br/>
		/// <br/>
		/// </summary>
		/// <param name="key">Cache key</param>
		void Remove(TKey key);

		/// <summary>
		/// Count all cached values<br/>
		/// <br/>
		/// </summary>
		/// <returns></returns>
		int Count();

		/// <summary>
		/// Clear all cached values<br/>
		/// <br/>
		/// </summary>
		void Clear();
	}
}
