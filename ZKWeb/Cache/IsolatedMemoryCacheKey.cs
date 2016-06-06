using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Cache {
	/// <summary>
	/// 支持按照指定策略隔离缓存数据使用的键类型
	/// </summary>
	/// <typeparam name="TKey">键类型</typeparam>
	public class IsolatedMemoryCacheKey<TKey> : IEquatable<IsolatedMemoryCacheKey<TKey>> {
		/// <summary>
		/// 缓存键
		/// </summary>
		public TKey Key { get; protected set; }
		/// <summary>
		/// 隔离键列表
		/// </summary>
		public IList<object> IsolationKeys { get; protected set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="key">缓存键</param>
		/// <param name="isolationKeys">隔离键列表</param>
		public IsolatedMemoryCacheKey(TKey key, IList<object> isolationKeys) {
			Key = key;
			IsolationKeys = isolationKeys ?? new List<object>();
		}

		/// <summary>
		/// 比较是否相等
		/// </summary>
		/// <param name="obj">比较的对象</param>
		/// <returns></returns>
		public bool Equals(IsolatedMemoryCacheKey<TKey> obj) {
			return (Key.EqualsSupportsNull(obj.Key) &&
				IsolationKeys.SequenceEqual(obj.IsolationKeys));
		}

		/// <summary>
		/// 比较是否相等
		/// </summary>
		/// <param name="obj">比较的对象</param>
		/// <returns></returns>
		public override bool Equals(object obj) {
			var cacheKey = obj as IsolatedMemoryCacheKey<TKey>;
			return (cacheKey != null) && Equals(cacheKey);
		}

		/// <summary>
		/// 获取Hash值
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
