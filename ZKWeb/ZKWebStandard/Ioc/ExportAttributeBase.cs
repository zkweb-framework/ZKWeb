using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Base attribute for register type to IoC container<br/>
	/// 注册类型到IoC容器使用的属性的基类<br/>
	/// </summary>
	public abstract class ExportAttributeBase : Attribute {
		/// <summary>
		/// Register implementation type to container<br/>
		/// 注册实现类型到容器<br/>
		/// </summary>
		/// <param name="container">Container</param>
		/// <param name="type">Implementation type</param>
		/// <param name="reuseType">Reuse type</param>
		public abstract void RegisterToContainer(IContainer container, Type type, ReuseType reuseType);
	}
}
