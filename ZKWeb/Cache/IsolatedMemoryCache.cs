using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Cache.Interfaces;
using ZKWeb.Utils.Collections;

namespace ZKWeb.Cache {
	/// <summary>
	/// 支持按照指定策略隔离缓存数据的类
	/// </summary>
	/// <typeparam name="TKey">键类型</typeparam>
	/// <typeparam name="TValue">值类型</typeparam>
	public class IsolatedMemoryCache<TKey, TValue> :
		MemoryCache<KeyValuePair<TKey, object>, TValue> {
		/// <summary>
		/// 当前使用的缓存隔离策略
		/// </summary>
		public ICacheIsolationPolicy IsolationPolicy { get; protected set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="isolationPolicyName">缓存隔离策略的名称，通过容器查找策略对象</param>
		public IsolatedMemoryCache(string isolationPolicyName) : this(
			string.IsNullOrEmpty(isolationPolicyName) ? null :
				Application.Ioc.Resolve<ICacheIsolationPolicy>(serviceKey: isolationPolicyName)) {
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="isolationPolicy">缓存隔离策略</param>
		public IsolatedMemoryCache(ICacheIsolationPolicy isolationPolicy) {
			IsolationPolicy = isolationPolicy;
		}

		/// <summary>
		/// 生成实际使用的缓存键
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public KeyValuePair<TKey, object> GenerateKey(TKey key) {
			return new KeyValuePair<TKey, object>(key, IsolationPolicy?.GetIsolationKey());
		}

		/// <summary>
		/// 设置缓存数据
		/// </summary>
		/// <param name="key">缓存键</param>
		/// <param name="value">缓存值</param>
		/// <param name="keepTime">缓存时间</param>
		public void Put(TKey key, TValue value, TimeSpan keepTime) {
			Put(GenerateKey(key), value, keepTime);
		}

		/// <summary>
		/// 获取缓存数据
		/// 没有或已过期时返回默认值
		/// </summary>
		/// <param name="key">缓存键</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns></returns>
		public TValue GetOrDefault(TKey key, TValue defaultValue = default(TValue)) {
			return GetOrDefault(GenerateKey(key), defaultValue);
		}

		/// <summary>
		/// 删除缓存数据
		/// </summary>
		/// <param name="key">缓存键</param>
		public void Remove(TKey key) {
			Remove(GenerateKey(key));
		}
	}
}
