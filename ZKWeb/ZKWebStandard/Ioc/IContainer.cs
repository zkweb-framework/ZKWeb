using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Interface for IoC container<br/>
	/// IoC容器的接口<br/>
	/// </summary>
	public interface IContainer :
		IRegistrator, IGenericRegistrator,
		IResolver, IGenericResolver, IDisposable {
		/// <summary>
		/// Clone the container<br/>
		/// 克隆容器<br/>
		/// </summary>
		/// <returns></returns>
		object Clone();
	}
}
