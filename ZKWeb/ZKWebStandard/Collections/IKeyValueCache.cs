using System;

namespace ZKWebStandard.Collections {
	/// <summary>
	/// Interface of key-value cache<br/>
	/// It should be thread safe<br/>
	/// 键值缓存的接口<br/>
	/// 它应该是线程安全的<br/>
	/// </summary>
	/// <seealso cref="MemoryCache{TKey, TValue}"/>
	public interface IKeyValueCache<TKey, TValue> {
		/// <summary>
		/// Put value to cache<br/>
		/// 插入值到缓存<br/>
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="value">Cache value</param>
		/// <param name="keepTime">Keep time</param>
		void Put(TKey key, TValue value, TimeSpan keepTime);

		/// <summary>
		/// Try to get cached value<br/>
		/// Return false if no exist value or exist value expired<br/>
		/// 尝试获取缓存制<br/>
		/// 如果值不存在或者已过期则返回false<br/>
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="value">Cache value</param>
		/// <returns></returns>
		bool TryGetValue(TKey key, out TValue value);

		/// <summary>
		/// Remove cached value<br/>
		/// 删除已缓存的值<br/>
		/// </summary>
		/// <param name="key">Cache key</param>
		void Remove(TKey key);

		/// <summary>
		/// Count all cached values<br/>
		/// 获取缓存值的数量<br/>
		/// </summary>
		/// <returns></returns>
		int Count();

		/// <summary>
		/// Clear all cached values<br/>
		/// 删除所有缓存值<br/>
		/// </summary>
		void Clear();
	}
}
