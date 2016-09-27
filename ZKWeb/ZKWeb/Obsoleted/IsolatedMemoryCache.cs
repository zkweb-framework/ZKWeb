using System;
using System.Collections.Generic;
using System.Linq;
using ZKWebStandard.Collections;

namespace ZKWeb.Cache {
	/// <summary>
	/// This class should no longer be using
	/// Please use ICacheFactory
	/// Obsleted in 1.0.2
	/// </summary>
	/// <typeparam name="TKey">Original key type</typeparam>
	/// <typeparam name="TValue">Value type</typeparam>
	[Obsolete("This class should no longer be using, Please use ICacheFactory")]
	public class IsolatedMemoryCache<TKey, TValue> : IsolatedKeyValueCache<TKey, TValue> {
		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="isolationPolicyNames">Cache isolation policy names, resolve by IoC container</param>
		public IsolatedMemoryCache(params string[] isolationPolicyNames) :
			this((IEnumerable<string>)isolationPolicyNames) { }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="isolationPolicyNames">Cache isolation policy names, resolve by IoC container</param>
		public IsolatedMemoryCache(IEnumerable<string> isolationPolicyNames) :
			this(isolationPolicyNames.Select(name =>
				Application.Ioc.Resolve<ICacheIsolationPolicy>(serviceKey: name)).ToList()) { }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="isolationPolicies">Cache isolation policies</param>
		public IsolatedMemoryCache(params ICacheIsolationPolicy[] isolationPolicies) :
			this((IList<ICacheIsolationPolicy>)isolationPolicies) { }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="isolationPolicies">Cache isolation policies</param>
		public IsolatedMemoryCache(IList<ICacheIsolationPolicy> isolationPolicies) :
			base(isolationPolicies, new MemoryCache<IsolatedCacheKey<TKey>, TValue>()) { }
	}
}
