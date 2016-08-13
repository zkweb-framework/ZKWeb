using System.Collections.Generic;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Interface for generic resolver
	/// </summary>
	public interface IGenericResolver {
		/// <summary>
		/// Resolve service with type and key
		/// Throw exception or return default value if not found, dependent on ifUnresolved
		/// </summary>
		/// <typeparam name="TService">Service type</typeparam>
		/// <param name="ifUnresolved">Action when service unresolved</param>
		/// <param name="serviceKey">Service key</param>
		/// <returns></returns>
		TService Resolve<TService>(IfUnresolved ifUnresolved = IfUnresolved.Throw, object serviceKey = null);

		/// <summary>
		/// Resolve services with type and key
		/// Return empty sequence if no service registered
		/// </summary>
		/// <typeparam name="TService">Service type</typeparam>
		/// <param name="serviceKey">Service key</param>
		/// <returns></returns>
		IEnumerable<TService> ResolveMany<TService>(object serviceKey = null);
	}
}
