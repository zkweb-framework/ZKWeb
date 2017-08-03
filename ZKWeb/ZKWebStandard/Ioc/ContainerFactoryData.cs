using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Factory data<br/>
	/// Include function object and implementation type<br/>
	/// 工厂函数数据<br/>
	/// 包括函数对象和实现类型<br/>
	/// </summary>
	/// <seealso cref="Container"/>
	public class ContainerFactoryData {
		/// <summary>
		/// Function returns implementation type<br/>
		/// 返回实现类型的函数<br/>
		/// </summary>
		public object GenericFactory { get; internal set; }
		/// <summary>
		/// Function returns object type<br/>
		/// 返回object类型的函数<br/>
		/// </summary>
		public Func<object> ObjectFactory { get; internal set; }
		/// <summary>
		/// Implementation type hint<br/>
		/// Usually it's the type factory function will return<br/>
		/// Except user use "RegisterDelegate", that we can't known what type will be returned<br/>
		/// 实现类型<br/>
		/// 通常它会是工厂函数返回的对象的类型<br/>
		/// 除非用户使用"RegisterDelegate", 我们不知道什么类型会被返回<br/>
		/// </summary>
		public Type ImplementationTypeHint { get; internal set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="genericFactory">Function returns implementation type</param>
		/// <param name="objectFactory">Function returns object type</param>
		/// <param name="implementationTypeHint">Implementation type hint</param>
		public ContainerFactoryData(object genericFactory, Func<object> objectFactory, Type implementationTypeHint) {
			GenericFactory = genericFactory;
			ObjectFactory = objectFactory;
			ImplementationTypeHint = implementationTypeHint;
		}
	}
}
