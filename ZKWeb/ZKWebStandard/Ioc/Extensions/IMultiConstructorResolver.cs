namespace ZKWebStandard.Ioc.Extensions {
	/// <summary>
	/// Interface used to resolve type have multi constructors<br/>
	/// 用于解决拥有多个构造函数的类型的接口<br/>
	/// </summary>
	public interface IMultiConstructorResolver {
		/// <summary>
		/// Resolve type have multi constructors<br/>
		/// 解决拥有多个构造函数的类型<br/>
		/// </summary>
		T Resolve<T>();

		/// <summary>
		/// Clear factory cache, please run it after alter container<br/>
		/// 清空工厂函数的缓存, 请在修改容器后运行它<br/>
		/// </summary>
		void ClearCache();
	}
}
