using System.Collections.Generic;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Interface for generic resolver<br/>
	/// <br/>
	/// </summary>
	public interface IGenericResolver {
		/// <summary>
		/// Resolve service with type and key<br/>
		/// Throw exception or return default value if not found, dependent on ifUnresolved<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="TService">Service type</typeparam>
		/// <param name="ifUnresolved">Action when service unresolved</param>
		/// <param name="serviceKey">Service key</param>
		/// <returns></returns>
		TService Resolve<TService>(IfUnresolved ifUnresolved = IfUnresolved.Throw, object serviceKey = null);

		/// <summary>
		/// Resolve services with type and key<br/>
		/// Return empty sequence if no service registered<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="TService">Service type</typeparam>
		/// <param name="serviceKey">Service key</param>
		/// <returns></returns>
		IEnumerable<TService> ResolveMany<TService>(object serviceKey = null);
	}
}
