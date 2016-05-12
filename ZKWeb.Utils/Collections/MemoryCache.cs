using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Utils.Collections {
	/// <summary>
	/// 缓存类，支持指定过期时间，线程安全
	/// </summary>
	/// <typeparam name="TKey">键类型</typeparam>
	/// <typeparam name="TValue">值类型</typeparam>
	public class MemoryCache<TKey, TValue> {
		/// <summary>
		/// 定期删除过期数据的间隔时间，秒
		/// </summary>
		public const int RevokeExpiresInterval = 180;
		/// <summary>
		/// 缓存数据
		/// 结构 { 键, (对象, 过期时间) }
		/// 使用了线程锁所以这里只需要普通的Dictionary
		/// </summary>
		internal IDictionary<TKey, Tuple<TValue, DateTime>> Cache { get; }
		= new Dictionary<TKey, Tuple<TValue, DateTime>>();
		/// <summary>
		/// 缓存数据的线程锁
		/// </summary>
		internal ReaderWriterLockSlim CacheLock { get; set; } =
			new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
		/// <summary>
		/// 最后一次删除过期缓存的时间
		/// </summary>
		internal DateTime LastRevokeExpires { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// 删除过期的缓存数据
		/// 固定每180秒一次
		/// </summary>
		void RevokeExpires() {
			var now = DateTime.UtcNow;
			if ((now - LastRevokeExpires).TotalSeconds < RevokeExpiresInterval) {
				return;
			}
			using (CacheLock.WithWriteLock()) {
				if ((now - LastRevokeExpires).TotalSeconds < RevokeExpiresInterval) {
					return; // double check
				}
				LastRevokeExpires = now;
				var expireKeys = Cache.Where(c => c.Value.Item2 < now).Select(c => c.Key).ToList();
				foreach (var key in expireKeys) {
					Cache.Remove(key);
				}
			}
		}

		/// <summary>
		/// 设置缓存数据
		/// </summary>
		/// <param name="key">缓存键</param>
		/// <param name="value">缓存值</param>
		/// <param name="keepTime">保留时间</param>
		public void Put(TKey key, TValue value, TimeSpan keepTime) {
			RevokeExpires();
			using (CacheLock.WithWriteLock()) {
				var now = DateTime.UtcNow;
				Cache[key] = Tuple.Create(value, now + keepTime);
			}
		}

		/// <summary>
		/// 获取缓存数据
		/// 没有或已过期时返回默认值
		/// </summary>
		/// <param name="key">缓存键</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns></returns>
		public TValue GetOrDefault(TKey key, TValue defaultValue = default(TValue)) {
			RevokeExpires();
			using (CacheLock.WithReadLock()) {
				var now = DateTime.UtcNow;
				var value = Cache.GetOrDefault(key);
				if (value != null && value.Item2 > now) {
					return value.Item1;
				}
				return defaultValue;
			}
		}

		/// <summary>
		/// 删除缓存数据
		/// </summary>
		/// <param name="key">缓存键</param>
		public void Remove(TKey key) {
			RevokeExpires();
			using (CacheLock.WithWriteLock()) {
				Cache.Remove(key);
			}
		}

		/// <summary>
		/// 清空缓存数据
		/// </summary>
		public void Clear() {
			using (CacheLock.WithWriteLock()) {
				Cache.Clear();
			}
		}
	}
}
