using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Interface for generic registrator
	/// </summary>
	public interface IGenericRegistrator {
		/// <summary>
		/// Register implementation type with service type and service key
		/// </summary>
		/// <typeparam name="TService">Service type</typeparam>
		/// <typeparam name="TImplementation">Implementation type</typeparam>
		/// <param name="reuseType">Reuse type</param>
		/// <param name="serviceKey">Service key</param>
		void Register<TService, TImplementation>(
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null);

		/// <summary>
		/// Register implementation type with service types and service key
		/// Service types are obtain from base types and interfaces
		/// </summary>
		/// <typeparam name="TImplementation">Implementation type</typeparam>
		/// <param name="reuseType">Reuse type</param>
		/// <param name="serviceKey">Service key</param>
		/// <param name="nonPublicServiceTypes">Also register with non public service types</param>
		void RegisterMany<TImplementation>(
			ReuseType reuseType = ReuseType.Transient,
			object serviceKey = null, bool nonPublicServiceTypes = false);

		/// <summary>
		/// Register instance with service type and service key
		/// Reuse type is forced to Singleton
		/// </summary>
		/// <typeparam name="TService">Service type</typeparam>
		/// <param name="instance">Service instance</param>
		/// <param name="serviceKey">Service key</param>
		void RegisterInstance<TService>(TService instance, object serviceKey = null);

		/// <summary>
		/// Register delegate with service type and service key
		/// </summary>
		/// <typeparam name="TService">Service type</typeparam>
		/// <param name="factory">Service facotory</param>
		/// <param name="reuseType">Reuse type</param>
		/// <param name="serviceKey">Service key</param>
		void RegisterDelegate<TService>(Func<TService> factory,
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null);

		/// <summary>
		/// Unregister all factories with specified service type and service key
		/// </summary>
		/// <typeparam name="TService">Service type</typeparam>
		/// <param name="serviceKey">Service key</param>
		void Unregister<TService>(object serviceKey = null);
	}
}
