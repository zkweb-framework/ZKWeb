using System;
using System.Collections.Generic;
using System.Linq;
using ZKWebStandard.Collections;

namespace ZKWeb.Cache {
	/// <summary>
	/// This class should no longer be used<br/>
	/// Please use ICacheFactory<br/>
	/// 这个类不应该再被使用, 请使用ICacheFactory<br/>
	/// </summary>
	/// <typeparam name="TKey">Original key type</typeparam>
	/// <typeparam name="TValue">Value type</typeparam>
	[Obsolete("This class should no longer be used, Please use ICacheFactory")]
	public class IsolatedMemoryCache<TKey, TValue> : IsolatedKeyValueCache<TKey, TValue> {
		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="isolationPolicyNames">Cache isolation policy names, resolve by IoC container</param>
		public IsolatedMemoryCache(params string[] isolationPolicyNames) :
			this((IEnumerable<string>)isolationPolicyNames) { }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="isolationPolicyNames">Cache isolation policy names, resolve by IoC container</param>
		public IsolatedMemoryCache(IEnumerable<string> isolationPolicyNames) :
			this(isolationPolicyNames.Select(name =>
				Application.Ioc.Resolve<ICacheIsolationPolicy>(serviceKey: name)).ToList()) { }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="isolationPolicies">Cache isolation policies</param>
		public IsolatedMemoryCache(params ICacheIsolationPolicy[] isolationPolicies) :
			this((IList<ICacheIsolationPolicy>)isolationPolicies) { }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="isolationPolicies">Cache isolation policies</param>
		public IsolatedMemoryCache(IList<ICacheIsolationPolicy> isolationPolicies) :
			base(isolationPolicies, new MemoryCache<IsolatedCacheKey<TKey>, TValue>()) { }
	}
}
