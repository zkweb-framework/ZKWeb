using System;
using System.Collections.Generic;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Interface for resolver<br/>
	/// <br/>
	/// </summary>
	public interface IResolver {
		/// <summary>
		/// Resolve service with type and key<br/>
		/// Throw exception or return default value if not found, dependent on ifUnresolved<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="serviceType">Service type</param>
		/// <param name="ifUnresolved">Action when service unresolved</param>
		/// <param name="serviceKey">Service key</param>
		/// <returns></returns>
		object Resolve(Type serviceType, IfUnresolved ifUnresolved = IfUnresolved.Throw, object serviceKey = null);

		/// <summary>
		/// Resolve services with type and key<br/>
		/// Return empty sequence if no service registered<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="serviceType">Service type</param>
		/// <param name="serviceKey">Service key</param>
		/// <returns></returns>
		IEnumerable<object> ResolveMany(Type serviceType, object serviceKey = null);
	}
}
