using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ZKWebStandard.Collections {
	/// <summary>
	/// Memory cache, support specific lifetime and thread safe
	/// </summary>
	/// <typeparam name="TKey">Key type</typeparam>
	/// <typeparam name="TValue">Value type</typeparam>
	public class MemoryCache<TKey, TValue> {
		/// <summary>
		/// Check interval for revoke expired values
		/// Default is 180s
		/// </summary>
		public TimeSpan RevokeExpiresInterval { get; set; }
		/// <summary>
		/// Cache
		/// { Key: (Value, ExpireTime) }
		/// </summary>
		protected IDictionary<TKey, Pair<TValue, DateTime>> Cache { get; set; }
		/// <summary>
		/// Reader writer lock
		/// </summary>
		protected ReaderWriterLockSlim CacheLock { get; set; }
		/// <summary>
		/// Last check time
		/// </summary>
		protected DateTime LastRevokeExpires { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public MemoryCache() {
			RevokeExpiresInterval = TimeSpan.FromSeconds(180);
			Cache = new Dictionary<TKey, Pair<TValue, DateTime>>();
			CacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
			LastRevokeExpires = DateTime.UtcNow;
		}

		/// <summary>
		/// Revoke expired values if the check interval has elapsed
		/// </summary>
		protected void RevokeExpires() {
			var now = DateTime.UtcNow;
			if ((now - LastRevokeExpires) < RevokeExpiresInterval) {
				return;
			}
			CacheLock.EnterWriteLock();
			try {
				if ((now - LastRevokeExpires) < RevokeExpiresInterval) {
					return; // double check
				}
				LastRevokeExpires = now;
				var expireKeys = Cache.Where(c => c.Value.Second < now).Select(c => c.Key).ToList();
				foreach (var key in expireKeys) {
					Cache.Remove(key);
				}
			} finally {
				CacheLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Put value to cache
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="value">Cache value</param>
		/// <param name="keepTime">Keep time</param>
		public void Put(TKey key, TValue value, TimeSpan keepTime) {
			RevokeExpires();
			if (keepTime == TimeSpan.Zero) {
				return;
			}
			var now = DateTime.UtcNow;
			CacheLock.EnterWriteLock();
			try {
				Cache[key] = Pair.Create(value, now + keepTime);
			} finally {
				CacheLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Try to get cached value
		/// Return false if no exist value or exist value expired
		/// </summary>
		/// <param name="key">Cache key</param>
		/// <param name="value">Cache value</param>
		/// <returns></returns>
		public bool TryGetValue(TKey key, out TValue value) {
			RevokeExpires();
			var now = DateTime.UtcNow;
			CacheLock.EnterReadLock();
			try {
				Pair<TValue, DateTime> pair;
				if (Cache.TryGetValue(key, out pair) && pair.Second > now) {
					value = pair.First;
					return true;
				} else {
					value = default(TValue);
					return false;
				}
			} finally {
				CacheLock.ExitReadLock();
			}
		}

		/// <summary>
		/// Get cached value
		/// Return default value if no exist value or exist value expired
		/// </summary>
		/// <param name="key">Key</param>
		/// <param name="defaultValue">The default value</param>
		/// <returns></returns>
		public TValue GetOrDefault(TKey key, TValue defaultValue = default(TValue)) {
			TValue value;
			if (TryGetValue(key, out value)) {
				return value;
			}
			return defaultValue;
		}

		/// <summary>
		/// Get cached value
		/// Generate a new value and store it to cache if the no exist value or exist value expired
		/// Attention: This is not an atomic operation
		/// </summary>
		/// <param name="key">缓存</param>
		/// <param name="creator">创建函数</param>
		/// <param name="keepTime">保留时间</param>
		/// <returns></returns>
		public TValue GetOrCreate(TKey key, Func<TValue> creator, TimeSpan keepTime) {
			TValue value;
			if (keepTime == TimeSpan.Zero || !TryGetValue(key, out value)) {
				value = creator();
				Put(key, value, keepTime);
			}
			return value;
		}

		/// <summary>
		/// Remove cached value
		/// </summary>
		/// <param name="key">Cache key</param>
		public void Remove(TKey key) {
			RevokeExpires();
			CacheLock.EnterWriteLock();
			try {
				Cache.Remove(key);
			} finally {
				CacheLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Count all cached values
		/// </summary>
		/// <returns></returns>
		public int Count() {
			CacheLock.EnterReadLock();
			try {
				return Cache.Count;
			} finally {
				CacheLock.ExitReadLock();
			}
		}

		/// <summary>
		/// Clear all cached values
		/// </summary>
		public void Clear() {
			CacheLock.EnterWriteLock();
			try {
				Cache.Clear();
			} finally {
				CacheLock.ExitWriteLock();
			}
		}
	}
}
