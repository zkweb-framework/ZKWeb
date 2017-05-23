using System;
using System.Collections.Generic;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Interface for registrator<br/>
	/// 注册器的接口<br/>
	/// </summary>
	/// <seealso cref="IContainer"/>
	/// <seealso cref="Container"/>
	public interface IRegistrator {
		/// <summary>
		/// Register implementation type with service type and service key<br/>
		/// 根据服务类型和服务键注册实现类型<br/>
		/// </summary>
		/// <param name="serviceType">Service type</param>
		/// <param name="implementationType">Implementation type</param>
		/// <param name="reuseType">Reuse type</param>
		/// <param name="serviceKey">Service key</param>
		void Register(Type serviceType, Type implementationType,
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null);

		/// <summary>
		/// Register implementation type with service types and service key<br/>
		/// 根据多个服务类型和服务键注册实现类型<br/>
		/// </summary>
		/// <param name="serviceTypes">Service types</param>
		/// <param name="implementationType">Implementation type</param>
		/// <param name="reuseType">Reuse type</param>
		/// <param name="serviceKey">Service key</param>
		void RegisterMany(IList<Type> serviceTypes, Type implementationType,
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null);

		/// <summary>
		/// Register implementation type with service types and service key<br/>
		/// Service types are obtain from base types and interfaces<br/>
		/// 根据多个服务类型和服务键注册实现类型<br/>
		/// 服务类型是实现类型的基类和接口<br/>
		/// </summary>
		/// <param name="implementationType">Implementation type</param>
		/// <param name="reuseType">Reuse type</param>
		/// <param name="serviceKey">Service key</param>
		/// <param name="nonPublicServiceTypes">Also register with non public service types</param>
		void RegisterMany(Type implementationType,
			ReuseType reuseType = ReuseType.Transient,
			object serviceKey = null, bool nonPublicServiceTypes = false);

		/// <summary>
		/// Register instance with service type and service key<br/>
		/// Reuse type is forced to Singleton<br/>
		/// 根据服务类型和服务键注册实例<br/>
		/// 重用类型强制为单例<br/>
		/// </summary>
		/// <param name="serviceType">Service type</param>
		/// <param name="instance">Service instance</param>
		/// <param name="serviceKey">Service key</param>
		void RegisterInstance(Type serviceType, object instance, object serviceKey = null);

		/// <summary>
		/// Register delegate with service type and service key<br/>
		/// 根据服务类型和服务键注册工厂函数<br/>
		/// </summary>
		/// <param name="serviceType">Implementation type</param>
		/// <param name="factory">Service factory</param>
		/// <param name="reuseType">Reuse type</param>
		/// <param name="serviceKey">Service key</param>
		void RegisterDelegate(Type serviceType, Func<object> factory,
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null);

		/// <summary>
		/// Automatic register types by export attributes<br/>
		/// 按Export属性自动注册类型到容器<br/>
		/// </summary>
		/// <param name="types">Implementation types</param>
		void RegisterExports(IEnumerable<Type> types);

		/// <summary>
		/// Unregister all factories with specified service type and service key<br/>
		/// For example:<br/>
		/// Class A inherit I and J<br/>
		/// Class B inherit I<br/>
		/// The factory mapping will be<br/>
		/// { A: A, I: [A, B], J: A, B: B }<br/>
		/// After unregister I the factory mapping will be<br/>
		/// { A: A, I: [], J: A, B: B }<br/>
		/// 注销所有注册到指定服务类型和服务键的工厂函数<br/>
		/// 例如:<br/>
		/// 类 A 继承 类 I 和 J<br/>
		/// 类 B 继承 类 I<br/>
		/// 工厂函数的集合是<br/>
		/// { A: A, I: [A, B], J: A, B: B }<br/>
		/// 按I注销后，工厂函数的集合是<br/>
		/// { A: A, I: [], J: A, B: B }<br/>
		/// </summary>
		/// <param name="serviceType">Service type</param>
		/// <param name="serviceKey">Service key</param>
		void Unregister(Type serviceType, object serviceKey = null);

		/// <summary>
		/// Unregister factories with specified implementation type and service key<br/>
		/// For example:<br/>
		/// Class A inherit I and J<br/>
		/// Class B inherit I<br/>
		/// The factory mapping will be<br/>
		/// { A: A, I: [A, B], J: A, B: B }<br/>
		/// After unregister A the factory mapping will be<br/>
		/// { A: [], I: [B], J: [], B: B }<br/>
		/// 注销所有注册到指定服务类型和服务键的工厂函数<br/>
		/// 例如:<br/>
		/// 类 A 继承 类 I 和 J<br/>
		/// 类 B 继承 类 I<br/>
		/// 工厂函数的集合是<br/>
		/// { A: A, I: [A, B], J: A, B: B }<br/>
		/// 按A注销后, 工厂函数的集合是<br/>
		/// { A: [], I: [B], J: [], B: B }<br/>
		/// </summary>
		/// <param name="implementationType">Implementation type</param>
		/// <param name="serviceKey">Service key</param>
		void UnregisterImplementation(Type implementationType, object serviceKey = null);

		/// <summary>
		/// Unregister all factories<br/>
		/// 注销所有工厂函数<br/>
		/// </summary>
		void UnregisterAll();
	}
}
