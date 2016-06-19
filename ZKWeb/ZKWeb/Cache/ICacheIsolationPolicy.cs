namespace ZKWeb.Cache {
	/// <summary>
	/// 缓存隔离策略
	/// 可以注册指定的名称到IoC容器中
	/// </summary>
	public interface ICacheIsolationPolicy {
		/// <summary>
		/// 获取隔离缓存使用的键
		/// </summary>
		/// <returns></returns>
		object GetIsolationKey();
	}
}
