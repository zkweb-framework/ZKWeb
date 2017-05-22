using System;
using System.Collections.Generic;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Interface for registrator<br/>
	/// <br/>
	/// </summary>
	public interface IRegistrator {
		/// <summary>
		/// Register implementation type with service type and service key<br/>
		/// <br/>
		/// </summary>
		/// <param name="serviceType">Service type</param>
		/// <param name="implementationType">Implementation type</param>
		/// <param name="reuseType">Reuse type</param>
		/// <param name="serviceKey">Service key</param>
		void Register(Type serviceType, Type implementationType,
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null);

		/// <summary>
		/// Register implementation type with service types and service key<br/>
		/// <br/>
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
		/// <br/>
		/// <br/>
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
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="serviceType">Service type</param>
		/// <param name="instance">Service instance</param>
		/// <param name="serviceKey">Service key</param>
		void RegisterInstance(Type serviceType, object instance, object serviceKey = null);

		/// <summary>
		/// Register delegate with service type and service key<br/>
		/// <br/>
		/// </summary>
		/// <param name="serviceType">Implementation type</param>
		/// <param name="factory">Service factory</param>
		/// <param name="reuseType">Reuse type</param>
		/// <param name="serviceKey">Service key</param>
		void RegisterDelegate(Type serviceType, Func<object> factory,
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null);

		/// <summary>
		/// Automatic register types by export attributes<br/>
		/// <br/>
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
		/// <br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// <br/>
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
		/// <br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="implementationType">Implementation type</param>
		/// <param name="serviceKey">Service key</param>
		void UnregisterImplementation(Type implementationType, object serviceKey = null);

		/// <summary>
		/// Unregister all factories<br/>
		/// <br/>
		/// </summary>
		void UnregisterAll();
	}
}
