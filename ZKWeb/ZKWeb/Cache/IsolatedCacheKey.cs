using System;
using System.Collections.Generic;
using System.Linq;
using ZKWebStandard.Extensions;

namespace ZKWeb.Cache {
	/// <summary>
	/// Cache key type that combines original cache key and isolation key
	/// Benchmark
	/// - struct Store 1.5s/1000000 times, Load 0.7s/1000000 times
	/// - class Store 2.0s/1000000 times, Load 0.7s/1000000 times
	/// </summary>
	/// <typeparam name="TKey">Original cache key type</typeparam>
	public struct IsolatedCacheKey<TKey> : IEquatable<IsolatedCacheKey<TKey>> {
		/// <summary>
		/// Original cache key
		/// </summary>
		public TKey Key { get; private set; }
		/// <summary>
		/// Isolation keys
		/// </summary>
		public IList<object> IsolationKeys { get; private set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="key">Original cache key</param>
		/// <param name="isolationKeys">Isolation keys</param>
		public IsolatedCacheKey(TKey key, IList<object> isolationKeys) {
			Key = key;
			IsolationKeys = isolationKeys ?? new List<object>();
		}

		/// <summary>
		/// Check if equals
		/// </summary>
		/// <param name="obj">Object compare to</param>
		/// <returns></returns>
		public bool Equals(IsolatedCacheKey<TKey> obj) {
			return (Key.EqualsSupportsNull(obj.Key) &&
				IsolationKeys.SequenceEqual(obj.IsolationKeys));
		}

		/// <summary>
		/// Check if equals
		/// </summary>
		/// <param name="obj">Object compare to</param>
		/// <returns></returns>
		public override bool Equals(object obj) {
			return (obj is IsolatedCacheKey<TKey> &&
				Equals((IsolatedCacheKey<TKey>)obj));
		}

		/// <summary>
		/// Generate hash value
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
