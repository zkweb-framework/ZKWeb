using System.Collections.Generic;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Interface for generic resolver<br/>
	/// 泛型解决器的接口<br/>
	/// </summary>
	/// <seealso cref="IContainer"/>
	/// <seealso cref="Container"/>
	public interface IGenericResolver {
		/// <summary>
		/// Resolve service with type and key<br/>
		/// Throw exception or return default value if not found, dependent on ifUnresolved<br/>
		/// 根据服务类型和服务键获取实例<br/>
		/// 找不到时根据ifUnresolved参数抛出例外或者返回默认值<br/>
		/// </summary>
		/// <typeparam name="TService">Service type</typeparam>
		/// <param name="ifUnresolved">Action when service unresolved</param>
		/// <param name="serviceKey">Service key</param>
		/// <returns></returns>
		TService Resolve<TService>(IfUnresolved ifUnresolved = IfUnresolved.Throw, object serviceKey = null);

		/// <summary>
		/// Resolve services with type and key<br/>
		/// Return empty sequence if no service registered<br/>
		/// 根据服务类型和服务键获取实例列表<br/>
		/// 如果无注册的服务则返回空列表<br/>
		/// </summary>
		/// <typeparam name="TService">Service type</typeparam>
		/// <param name="serviceKey">Service key</param>
		/// <returns></returns>
		IEnumerable<TService> ResolveMany<TService>(object serviceKey = null);

		/// <summary>
		/// Retrive factories with type and key<br/>
		/// Return empty sequence if no service registered<br/>
		/// 根据服务类型和服务键获取工厂函数<br/>
		/// 如果无注册的服务则返回空列表<br/>
		/// </summary>
		/// <typeparam name="TService">Service type</typeparam>
		/// <param name="serviceKey">Service key</param>
		/// <returns></returns>
		IEnumerable<ContainerFactoryData> ResolveFactories<TService>(object serviceKey = null);
	}
}
