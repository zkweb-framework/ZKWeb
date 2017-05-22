using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Interface for IoC container<br/>
	/// <br/>
	/// </summary>
	public interface IContainer :
		IRegistrator, IGenericRegistrator,
		IResolver, IGenericResolver, IDisposable {
		/// <summary>
		/// Clone the container<br/>
		/// <br/>
		/// </summary>
		/// <returns></returns>
		object Clone();
	}
}
