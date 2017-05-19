using System;
using System.Collections.Generic;
using System.Linq;
using ZKWebStandard.Extensions;

namespace ZKWeb.Cache {
	/// <summary>
	/// Cache key type that combines original cache key and isolation key<br/>
	/// 缓存键类型，用于合并原始缓存键和隔离键<br/>
	/// Benchmark (性能测试)<br/>
	/// - struct Store 1.5s/1000000 times, Load 0.7s/1000000 times<br/>
	/// - class Store 2.0s/1000000 times, Load 0.7s/1000000 times<br/>
	/// </summary>
	/// <typeparam name="TKey">Original cache key type</typeparam>
	public struct IsolatedCacheKey<TKey> : IEquatable<IsolatedCacheKey<TKey>> {
		/// <summary>
		/// Original cache key<br/>
		/// 原始缓存键<br/>
		/// </summary>
		public TKey Key { get; private set; }
		/// <summary>
		/// Isolation keys<br/>
		/// 隔离键列表<br/>
		/// </summary>
		public IList<object> IsolationKeys { get; private set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="key">Original cache key</param>
		/// <param name="isolationKeys">Isolation keys</param>
		public IsolatedCacheKey(TKey key, IList<object> isolationKeys) {
			Key = key;
			IsolationKeys = isolationKeys ?? new List<object>();
		}

		/// <summary>
		/// Check whether the instances are equal<br/>
		/// 检查实例是否相等<br/>
		/// </summary>
		/// <param name="obj">Object compare to</param>
		/// <returns></returns>
		public bool Equals(IsolatedCacheKey<TKey> obj) {
			return (Key.EqualsSupportsNull(obj.Key) &&
				IsolationKeys.SequenceEqual(obj.IsolationKeys));
		}

		/// <summary>
		/// Check whether the instances are equal<br/>
		/// 检查实例是否相等<br/>
		/// </summary>
		/// <param name="obj">Object compare to</param>
		/// <returns></returns>
		public override bool Equals(object obj) {
			return (obj is IsolatedCacheKey<TKey> &&
				Equals((IsolatedCacheKey<TKey>)obj));
		}

		/// <summary>
		/// Generate hash of this instance<br/>
		/// 生成这个实例的哈希值<br/>
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() {
			int code = Key?.GetHashCode() ?? 0;
			foreach (var isolationKey in IsolationKeys) {
				code ^= isolationKey?.GetHashCode() ?? 0;
			}
			return code;
		}
	}
}
