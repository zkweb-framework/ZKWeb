using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Factory data<br/>
	/// Include function object and implementation type<br/>
	/// <br/>
	/// <br/>
	/// </summary>
	public struct ContainerFactoryData {
		/// <summary>
		/// Function object<br/>
		/// <br/>
		/// </summary>
		public Func<object> Factory { get; set; }
		/// <summary>
		/// Implementation type hint<br/>
		/// Usually it's the type factory function will return<br/>
		/// Except user use `RegisterDelegate`, that we can't known what type will be returned<br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// </summary>
		public Type ImplementationTypeHint { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		/// <param name="factory">Function object</param>
		/// <param name="implementationTypeHint">Implementation type hint</param>
		public ContainerFactoryData(Func<object> factory, Type implementationTypeHint) {
			Factory = factory;
			ImplementationTypeHint = implementationTypeHint;
		}
	}
}
