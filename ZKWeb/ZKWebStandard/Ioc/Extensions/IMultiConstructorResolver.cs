namespace ZKWebStandard.Ioc.Extensions {
	/// <summary>
	/// Interface used to resolve type have multi constructors
	/// </summary>
	public interface IMultiConstructorResolver {
		/// <summary>
		/// Resolve type have multi constructors
		/// </summary>
		T Resolve<T>(IContainer container);
	}
}
