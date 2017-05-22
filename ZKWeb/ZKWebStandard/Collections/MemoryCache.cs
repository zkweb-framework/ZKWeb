using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ZKWebStandard.Collections {
	/// <summary>
	/// Key-value cache based on memory<br/>
	/// 基于内存的键值缓存<br/>
	/// </summary>
	/// <typeparam name="TKey">Key type</typeparam>
	/// <typeparam name="TValue">Value type</typeparam>
	/// <seealso cref="IKeyValueCache{TKey, TValue}"/>
	/// <example>
	/// var cache = new MemoryCache&lt;int, string&gt;();
	/// cache.Put(1, "value of 1", TimeSpan.FromSeconds(100));
	/// 
	/// string cached;
	/// if (cache.TryGetValue(1, out cached)) {
	///		Console.WriteLine("cache hit: " + cached);
	/// }
	/// </example>
	public class MemoryCache<TKey, TValue> : IKeyValueCache<TKey, TValue> {
		/// <summary>
		/// Check interval for revoke expired values<br/>
		/// 删除已过期值的检查间隔<br/>
		/// Default is 180s
		/// </summary>
		public TimeSpan RevokeExpiresInterval { get; set; }
		/// <summary>
		/// Cache<br/>
		/// 缓存词典<br/>
		/// { Key: (Value, ExpireTime) }
		/// </summary>
		protected IDictionary<TKey, Pair<TValue, DateTime>> Cache { get; set; }
		/// <summary>
		/// Reader writer lock<br/>
		/// 读写锁<br/>
		/// </summary>
		protected ReaderWriterLockSlim CacheLock { get; set; }
		/// <summary>
		/// Last check time<br/>
		/// 上次检查的时间<br/>
		/// </summary>
		protected DateTime LastRevokeExpires { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public MemoryCache() {
			RevokeExpiresInterval = TimeSpan.FromSeconds(180);
			Cache = new Dictionary<TKey, Pair<TValue, DateTime>>();
			CacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
			LastRevokeExpires = DateTime.UtcNow;
		}

		/// <summary>
		/// Revoke expired values if the check interval has elapsed<br/>
		/// 删除已过期的值如果检查间隔已到<br/>
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
		/// Put value to cache<br/>
		/// 插入值到缓存中<br/>
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
		/// Try to get cached value<br/>
		/// Return false if no exist value or exist value expired<br/>
		/// 尝试获取已缓存的值<br/>
		/// 如果值不存在或已过期则返回false<br/>
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
		/// Remove cached value<br/>
		/// 删除已缓存的值<br/>
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
		/// Count all cached values<br/>
		/// 获取已缓存的值数量<br/>
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
		/// Clear all cached values<br/>
		/// 删除所有已缓存的值<br/>
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
