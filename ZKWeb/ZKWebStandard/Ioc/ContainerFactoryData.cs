using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Factory data
	/// Include function object and implementation type
	/// </summary>
	public struct ContainerFactoryData {
		/// <summary>
		/// Function object
		/// </summary>
		public Func<object> Factory { get; set; }
		/// <summary>
		/// Implementation type hint
		/// Usually it's the type factory function will return
		/// Except user use `RegisterDelegate`, that we can't known what type will be returned
		/// </summary>
		public Type ImplementationTypeHint { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="factory">Function object</param>
		/// <param name="implementationTypeHint">Implementation type hint</param>
		public ContainerFactoryData(Func<object> factory, Type implementationTypeHint) {
			Factory = factory;
			ImplementationTypeHint = implementationTypeHint;
		}
	}
}
