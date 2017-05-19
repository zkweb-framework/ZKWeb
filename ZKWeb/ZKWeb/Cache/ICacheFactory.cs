using ZKWebStandard.Collections;

namespace ZKWeb.Cache {
	/// <summary>
	/// Factory interface used to generate a key-value cache instance<br/>
	/// 用于生成一个键值缓存实例的工厂接口<br/>
	/// </summary>
	/// <example>
	/// <code language="cs">
	/// var cacheFactory = Application.Ioc.Resolve&lt;ICacheFactory&gt;();
	/// 
	/// // create simple key value cache
	/// var simpleCache = cacheFactory.CreateCache&lt;int, int&gt;();
	/// 
	/// // create device isolated key value cache
	/// var options = CacheFactoryOptions.Default.WithIsolationPolicies("Device");
	/// var isolatedCache = cacheFactory.CreateCache&lt;int, int&gt;(options);
	/// </code>
	/// </example>
	/// <seealso cref="CacheFactoryOptions"/>
	public interface ICacheFactory {
		/// <summary>
		/// Create cache instance<br/>
		/// 创建缓存实例<br/>
		/// </summary>
		/// <typeparam name="TKey">Key type</typeparam>
		/// <typeparam name="TValue">Value type</typeparam>
		/// <param name="options">Cache factory options</param>
		/// <returns></returns>
		IKeyValueCache<TKey, TValue> CreateCache<TKey, TValue>(
			CacheFactoryOptions options = null);
	}
}
