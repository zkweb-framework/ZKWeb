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
		MemoryCache<IsolatedMemoryCacheKey<TKey>, TValue> {
		/// <summary>
		/// 当前使用的缓存隔离策略列表
		/// </summary>
		public IList<ICacheIsolationPolicy> IsolationPolicies { get; protected set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="isolationPolicyNames">缓存隔离策略的名称列表，通过Ioc容器查找策略对象</param>
		public IsolatedMemoryCache(params string[] isolationPolicyNames) :
			this((IEnumerable<string>)isolationPolicyNames) { }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="isolationPolicyNames">缓存隔离策略的名称列表，通过Ioc容器查找策略对象</param>
		public IsolatedMemoryCache(IEnumerable<string> isolationPolicyNames) :
			this(isolationPolicyNames.Select(name =>
				Application.Ioc.Resolve<ICacheIsolationPolicy>(serviceKey: name)).ToList()) { }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="isolationPolicies">缓存隔离策略列表</param>
		public IsolatedMemoryCache(params ICacheIsolationPolicy[] isolationPolicies) :
			this((IList<ICacheIsolationPolicy>)isolationPolicies) { }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="isolationPolicies">缓存隔离策略列表</param>
		public IsolatedMemoryCache(IList<ICacheIsolationPolicy> isolationPolicies) {
			IsolationPolicies = isolationPolicies;
		}

		/// <summary>
		/// 生成实际使用的缓存键
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public IsolatedMemoryCacheKey<TKey> GenerateKey(TKey key) {
			var isolationKeys = new List<object>(IsolationPolicies.Count);
			foreach (var policy in IsolationPolicies) {
				isolationKeys.Add(policy.GetIsolationKey());
			}
			return new IsolatedMemoryCacheKey<TKey>(key, isolationKeys);
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
