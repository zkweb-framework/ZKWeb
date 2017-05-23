using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Factory data<br/>
	/// Include function object and implementation type<br/>
	/// 工厂函数数据<br/>
	/// 包括函数对象和实现类型<br/>
	/// </summary>
	public struct ContainerFactoryData {
		/// <summary>
		/// Function object<br/>
		/// 函数对象<br/>
		/// </summary>
		public Func<object> Factory { get; set; }
		/// <summary>
		/// Implementation type hint<br/>
		/// Usually it's the type factory function will return<br/>
		/// Except user use "RegisterDelegate", that we can't known what type will be returned<br/>
		/// 实现类型<br/>
		/// 通常它会是工厂函数返回的对象的类型<br/>
		/// 除非用户使用"RegisterDelegate", 我们不知道什么类型会被返回<br/>
		/// </summary>
		public Type ImplementationTypeHint { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="factory">Function object</param>
		/// <param name="implementationTypeHint">Implementation type hint</param>
		public ContainerFactoryData(Func<object> factory, Type implementationTypeHint) {
			Factory = factory;
			ImplementationTypeHint = implementationTypeHint;
		}
	}
}
