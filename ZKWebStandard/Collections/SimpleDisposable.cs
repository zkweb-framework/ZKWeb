using System;
using System.Threading;

namespace ZKWebStandard.Collections {
	/// <summary>
	/// 简单的Disposable类
	/// 可以指定在对象回收时执行的函数
	/// 指定的函数最多只会被执行一次
	/// </summary>
	public class SimpleDisposable : IDisposable {
		/// <summary>
		/// 在Dispose时执行的函数
		/// </summary>
		protected Action OnDispose { get; set; }
		/// <summary>
		/// 是否已执行过Dispose
		/// </summary>
		protected int Disposed = 0;

		/// <summary>
		/// 初始化
		/// </summary>
		public SimpleDisposable(Action onDispose) {
			OnDispose = onDispose;
		}

		/// <summary>
		/// 执行释放函数
		/// </summary>
		public void Dispose() {
			if (Interlocked.Exchange(ref Disposed, 1) == 0) {
				OnDispose();
			}
		}

		/// <summary>
		/// 执行释放函数
		/// </summary>
		~SimpleDisposable() {
			Dispose();
		}
	}
}
