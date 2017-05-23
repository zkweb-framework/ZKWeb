using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Interface for generic registrator<br/>
	/// 泛型注册器的接口<br/>
	/// </summary>
	/// <seealso cref="IContainer"/>
	/// <seealso cref="Container"/>
	public interface IGenericRegistrator {
		/// <summary>
		/// Register implementation type with service type and service key<br/>
		/// 根据服务类型和服务键注册实现类型<br/>
		/// </summary>
		/// <typeparam name="TService">Service type</typeparam>
		/// <typeparam name="TImplementation">Implementation type</typeparam>
		/// <param name="reuseType">Reuse type</param>
		/// <param name="serviceKey">Service key</param>
		void Register<TService, TImplementation>(
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null);

		/// <summary>
		/// Register implementation type with service types and service key<br/>
		/// Service types are obtain from base types and interfaces<br/>
		/// 根据多个服务类型和服务键注册实现类型<br/>
		/// 服务类型是实现类型的基类和接口<br/>
		/// </summary>
		/// <typeparam name="TImplementation">Implementation type</typeparam>
		/// <param name="reuseType">Reuse type</param>
		/// <param name="serviceKey">Service key</param>
		/// <param name="nonPublicServiceTypes">Also register with non public service types</param>
		void RegisterMany<TImplementation>(
			ReuseType reuseType = ReuseType.Transient,
			object serviceKey = null, bool nonPublicServiceTypes = false);

		/// <summary>
		/// Register instance with service type and service key<br/>
		/// Reuse type is forced to Singleton<br/>
		/// 根据服务类型和服务键注册实例<br/>
		/// 重用类型强制为单例<br/>
		/// </summary>
		/// <typeparam name="TService">Service type</typeparam>
		/// <param name="instance">Service instance</param>
		/// <param name="serviceKey">Service key</param>
		void RegisterInstance<TService>(TService instance, object serviceKey = null);

		/// <summary>
		/// Register delegate with service type and service key<br/>
		/// 根据服务类型和服务键注册工厂函数<br/>
		/// </summary>
		/// <typeparam name="TService">Service type</typeparam>
		/// <param name="factory">Service facotory</param>
		/// <param name="reuseType">Reuse type</param>
		/// <param name="serviceKey">Service key</param>
		void RegisterDelegate<TService>(Func<TService> factory,
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null);

		/// <summary>
		/// Unregister all factories with specified service type and service key<br/>
		/// Please see the example in IRegistrator<br/>
		/// 注销所有注册到指定服务类型和服务键的工厂函数<br/>
		/// 请查看IRegistrator中的示例<br/>
		/// </summary>
		/// <typeparam name="TService">Service type</typeparam>
		/// <param name="serviceKey">Service key</param>
		void Unregister<TService>(object serviceKey = null);

		/// <summary>
		/// Unregister factories with specified implementation type and service key<br/>
		/// Please see the example in IRegistrator<br/>
		/// 按实现类型和服务键注销所有相关的工厂函数<br/>
		/// 请查看IRegistrator中的示例<br/>
		/// </summary>
		/// <typeparam name="TImplementation">Implementation type</typeparam>
		/// <param name="serviceKey">Service key</param>
		void UnregisterImplementation<TImplementation>(object serviceKey = null);
	}
}
