using System.Collections.Generic;
using System.Linq;

namespace ZKWeb.Cache {
	/// <summary>
	/// Cache factory options
	/// </summary>
	public class CacheFactoryOptions {
		/// <summary>
		/// Cache lifetime
		/// </summary>
		public CacheLifetime Lifetime { get; set; }
		/// <summary>
		/// Cache isolation policies
		/// </summary>
		public IList<ICacheIsolationPolicy> IsolationPolicies { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public CacheFactoryOptions() {
			Lifetime = CacheLifetime.Singleton;
			IsolationPolicies = new List<ICacheIsolationPolicy>();
		}

		/// <summary>
		/// Set lifetime and return self
		/// </summary>
		/// <param name="lifetime">Cache lifetime</param>
		/// <returns></returns>
		public CacheFactoryOptions WithLifetime(CacheLifetime lifetime) {
			Lifetime = lifetime;
			return this;
		}

		/// <summary>
		/// Set isolation policies and return self
		/// </summary>
		/// <param name="isolationPolicyNames">Isolation policy names</param>
		/// <returns></returns>
		public CacheFactoryOptions WithIsolationPolicies(params string[] isolationPolicyNames) {
			return WithIsolationPolicies((IEnumerable<string>)isolationPolicyNames);
		}

		/// <summary>
		/// Set isolation policies and return self
		/// </summary>
		/// <param name="isolationPolicyNames">Isolation policy names</param>
		/// <returns></returns>
		public CacheFactoryOptions WithIsolationPolicies(IEnumerable<string> isolationPolicyNames) {
			return WithIsolationPolicies(isolationPolicyNames.Select(name =>
				Application.Ioc.Resolve<ICacheIsolationPolicy>(serviceKey: name)).ToList());
		}

		/// <summary>
		/// Set isolation policies and return self
		/// </summary>
		/// <param name="isolationPolicies">Isolation policies</param>
		/// <returns></returns>
		public CacheFactoryOptions WithIsolationPolicies(params ICacheIsolationPolicy[] isolationPolicies) {
			return WithIsolationPolicies((IList<ICacheIsolationPolicy>)isolationPolicies);
		}

		/// <summary>
		/// Set isolation policies and return self
		/// </summary>
		/// <param name="isolationPolicies">Isolation policies</param>
		/// <returns></returns>
		public CacheFactoryOptions WithIsolationPolicies(IList<ICacheIsolationPolicy> isolationPolicies) {
			IsolationPolicies = isolationPolicies;
			return this;
		}

		/// <summary>
		/// Get default options
		/// </summary>
		public static CacheFactoryOptions Default { get { return new CacheFactoryOptions(); } }
	}

	/// <summary>
	/// Cache lifetime
	/// </summary>
	public enum CacheLifetime {
		/// <summary>
		/// Singleton
		/// </summary>
		Singleton = 0,
		/// <summary>
		/// Per http context
		/// </summary>
		PerHttpContext = 1,
	}
}
