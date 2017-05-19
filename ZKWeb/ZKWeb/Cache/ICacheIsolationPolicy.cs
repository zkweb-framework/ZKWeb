namespace ZKWeb.Cache {
	/// <summary>
	/// Cache isolation policy<br/>
	/// 缓存隔离策略<br/>
	/// </summary>
	/// <example>
	/// If you need to select a policy by name, please provide the key when registering with the IoC container<br/>
	/// 如果你需要按名称来选择策略，请在注册到IoC容器时提供key<br/>
	/// <code language="cs">
	/// public class CacheIsolateByDevice : ICacheIsolationPolicy {
	///		public object GetIsolationKey() {
	///			return HttpManager.CurrentContext.GetClientDevice();
	///		}
	/// }
	/// 
	/// Application.Ioc.RegisterMany&lt;CacheIsolateByDevice&gt;(ReuseType.Singleton, serviceKey: "Device");
	/// </code>
	/// </example>
	/// <seealso cref="CacheFactory"/>
	/// <seealso cref="CacheFactoryOptions"/>
	public interface ICacheIsolationPolicy {
		/// <summary>
		/// Get isolation key<br/>
		/// 获取隔离键<br/>
		/// </summary>
		/// <returns></returns>
		object GetIsolationKey();
	}
}
