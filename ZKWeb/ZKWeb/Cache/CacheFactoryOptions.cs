using System.Collections.Generic;
using System.Linq;

namespace ZKWeb.Cache {
	/// <summary>
	/// Options used to generate the cache instance<br/>
	/// 用于生成缓存实例的参数<br/>
	/// </summary>
	/// <example>
	/// <code language="cs">
	/// var options = CacheFactoryOptions.Default.WithIsolationPolicies("Device");
	/// </code>
	/// </example>
	/// <seealso cref="CacheFactory"/>
	public class CacheFactoryOptions {
		/// <summary>
		/// The lifecycle of the cache<br/>
		/// 缓存的生命周期<br/>
		/// </summary>
		public CacheLifetime Lifetime { get; set; }
		/// <summary>
		/// The isolation policies of the cache<br/>
		/// 缓存的隔离策略<br/>
		/// </summary>
		public IList<ICacheIsolationPolicy> IsolationPolicies { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public CacheFactoryOptions() {
			Lifetime = CacheLifetime.Singleton;
			IsolationPolicies = new List<ICacheIsolationPolicy>();
		}

		/// <summary>
		/// Set the lifecycle of the cache<br/>
		/// 设置缓存的生命周期<br/>
		/// </summary>
		/// <param name="lifetime">Cache lifetime</param>
		/// <returns></returns>
		public CacheFactoryOptions WithLifetime(CacheLifetime lifetime) {
			Lifetime = lifetime;
			return this;
		}

		/// <summary>
		/// Set the isolation policies of the cache by their names<br/>
		/// 根据策略的名称设置缓存的隔离策略<br/>
		/// </summary>
		/// <param name="isolationPolicyNames">Isolation policy names</param>
		/// <returns></returns>
		public CacheFactoryOptions WithIsolationPolicies(params string[] isolationPolicyNames) {
			return WithIsolationPolicies((IEnumerable<string>)isolationPolicyNames);
		}

		/// <summary>
		/// Set the isolation policies of the cache by their names<br/>
		/// 根据策略的名称设置缓存的隔离策略<br/>
		/// </summary>
		/// <param name="isolationPolicyNames">Isolation policy names</param>
		/// <returns></returns>
		public CacheFactoryOptions WithIsolationPolicies(IEnumerable<string> isolationPolicyNames) {
			return WithIsolationPolicies(isolationPolicyNames.Select(name =>
				Application.Ioc.Resolve<ICacheIsolationPolicy>(serviceKey: name)).ToList());
		}

		/// <summary>
		/// Set the isolation policies of the cache by their instances<br/>
		/// 根据策略的实例设置缓存的隔离策略<br/>
		/// </summary>
		/// <param name="isolationPolicies">Isolation policies</param>
		/// <returns></returns>
		public CacheFactoryOptions WithIsolationPolicies(params ICacheIsolationPolicy[] isolationPolicies) {
			return WithIsolationPolicies((IList<ICacheIsolationPolicy>)isolationPolicies);
		}

		/// <summary>
		/// Set the isolation policies of the cache by their instances<br/>
		/// 根据策略的实例设置缓存的隔离策略<br/>
		/// </summary>
		/// <param name="isolationPolicies">Isolation policies</param>
		/// <returns></returns>
		public CacheFactoryOptions WithIsolationPolicies(IList<ICacheIsolationPolicy> isolationPolicies) {
			IsolationPolicies = isolationPolicies;
			return this;
		}

		/// <summary>
		/// Get default options<br/>
		/// 获取默认参数<br/>
		/// </summary>
		public static CacheFactoryOptions Default { get { return new CacheFactoryOptions(); } }
	}

	/// <summary>
	/// The lifecycle of the cache<br/>
	/// 缓存的生命周期<br/>
	/// </summary>
	public enum CacheLifetime {
		/// <summary>
		/// Singleton<br/>
		/// 单例<br/>
		/// </summary>
		Singleton = 0,
		/// <summary>
		/// Follow the http context<br/>
		/// 跟随Http上下文<br/>
		/// </summary>
		PerHttpContext = 1,
	}
}
