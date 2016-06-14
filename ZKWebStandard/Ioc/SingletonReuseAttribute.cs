using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// 设置使用单例模式的属性
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class SingletonReuseAttribute : ReuseAttribute {
		/// <summary>
		/// 初始化
		/// </summary>
		public SingletonReuseAttribute() : base(ReuseType.Singleton) { }
	}
}
