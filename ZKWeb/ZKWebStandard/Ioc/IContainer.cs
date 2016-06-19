using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Ioc容器的接口
	/// </summary>
	public interface IContainer :
		IRegistrator, IGenericRegistrator,
		IResolver, IGenericResolver,
		ICloneable, IDisposable {
	}
}
