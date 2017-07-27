using System;
using System.Collections.Generic;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWebStandard.Collections {
	/// <summary>
	/// Key-value cache based on http context<br/>
	/// All cached value will be destroy after context end<br/>
	/// 基于Http上下文的键值缓存<br/>
	/// 所有缓存值都会随着上下文的结束销毁<br/>
	/// </summary>
	/// <typeparam name="TKey">Key type</typeparam>
	/// <typeparam name="TValue">Value type</typeparam>
	public class HttpContextCache<TKey, TValue> : IKeyValueCache<TKey, TValue> {
		/// <summary>
		/// Http context key<br/>
		/// Http上下文中的键<br/>
		/// </summary>
		public object ContextKey { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public HttpContextCache() {
			ContextKey = new object();
		}

		/// <summary>
		/// Get cache storage from http context<br/>
		/// Use normal dictionary because thread race is impossible<br/>
		/// 获取缓存的储存对象<br/>
		/// 使用普通的词典, 因为线程冲突不可能发生<br/>
		/// </summary>
		/// <returns></returns>
		protected IDictionary<TKey, TValue> GetStorage() {
			return (IDictionary<TKey, TValue>)HttpManager.CurrentContext.Items
				.GetOrCreate(ContextKey, () => new Dictionary<TKey, TValue>());
		}

		/// <summary>
		/// Put value to cache<br/>
		/// 插入值到缓存<br/>
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="value">Cache value</param>
		/// <param name="keepTime">Keep time, not used</param>
		public void Put(TKey key, TValue value, TimeSpan keepTime) {
			if (!HttpManager.CurrentContextExists) {
				return;
			}
			var storage = GetStorage();
			storage[key] = value;
		}

		/// <summary>
		/// Try to get cached value<br/>
		/// Return false if no exist value or exist value expired<br/>
		/// 尝试从缓存获取值<br/>
		/// 如果值不存在或已过期则返回false<br/>
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="value">Cache value</param>
		/// <returns></returns>
		public bool TryGetValue(TKey key, out TValue value) {
			if (!HttpManager.CurrentContextExists) {
				value = default(TValue);
				return false;
			}
			var storage = GetStorage();
			return storage.TryGetValue(key, out value);
		}

		/// <summary>
		/// Remove cached value<br/>
		/// 删除缓存值<br/>
		/// </summary>
		/// <param name="key">Cache key</param>
		public void Remove(TKey key) {
			if (!HttpManager.CurrentContextExists) {
				return;
			}
			var storage = GetStorage();
			storage.Remove(key);
		}

		/// <summary>
		/// Count all cached values<br/>
		/// 返回缓存值的数量<br/>
		/// </summary>
		/// <returns></returns>
		public int Count() {
			if (!HttpManager.CurrentContextExists) {
				return 0;
			}
			var storage = GetStorage();
			return storage.Count;
		}

		/// <summary>
		/// Clear all cached values<br/>
		/// 清理所有缓存值<br/>
		/// </summary>
		public void Clear() {
			if (!HttpManager.CurrentContextExists) {
				return;
			}
			var storage = GetStorage();
			storage.Clear();
		}
	}
}
