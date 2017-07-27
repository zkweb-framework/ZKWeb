using System;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Interface for dispose objects when scope finished<br/>
	/// 用于在范围结束后调用指定对象的Dispose函数的接口<br/>
	/// </summary>
	public interface IScopeDisposer {
		/// <summary>
		/// Dispose specific object when scope finished<br/>
		/// 在范围结束后调用指定对象的Dispose函数<br/>
		/// </summary>
		void DisposeWhenScopeFinished(IDisposable disposable);

		/// <summary>
		/// Notify scope finished<br/>
		/// 通知当前范围已结束<br/>
		/// </summary>
		void ScopeFinished();
	}
}
