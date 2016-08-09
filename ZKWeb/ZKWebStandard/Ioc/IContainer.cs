using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Interface for IoC container
	/// </summary>
	public interface IContainer :
		IRegistrator, IGenericRegistrator,
		IResolver, IGenericResolver, IDisposable {
		/// <summary>
		/// Clone the container
		/// </summary>
		/// <returns></returns>
		object Clone();
	}
}
