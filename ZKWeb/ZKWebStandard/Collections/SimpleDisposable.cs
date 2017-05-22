using System;
using System.Threading;

namespace ZKWebStandard.Collections {
	/// <summary>
	/// Simple dispose object<br/>
	/// Execute the given method when disposed,<br/>
	/// The function will only be executed once<br/>
	/// 简单的可销毁对象<br/>
	/// 在销毁时执行指定的函数<br/>
	/// 函数只会被执行一次<br/>
	/// </summary>
	/// <example>
	/// <code>
	/// using (new SimpleDisposable(() =&gt; Console.WriteLine("release resources")) {
	///		Console.WriteLine("using resources");
	/// }
	/// </code>
	/// </example>
	public class SimpleDisposable : IDisposable {
		/// <summary>
		/// The function that is called when dispose<br/>
		/// 在销毁时调用的函数<br/>
		/// </summary>
		protected Action OnDispose { get; set; }
		/// <summary>
		/// Is method executed<br/>
		/// 函数是否已执行<br/>
		/// </summary>
		protected int Disposed = 0;

		/// <summary>
		/// Initialized<br/>
		/// 初始化<br/>
		/// </summary>
		public SimpleDisposable(Action onDispose) {
			OnDispose = onDispose;
		}

		/// <summary>
		/// Call the method if it's not called before<br/>
		/// 调用函数, 如果函数之前未被执行<br/>
		/// </summary>
		public void Dispose() {
			if (Interlocked.Exchange(ref Disposed, 1) == 0) {
				OnDispose();
			}
		}

		/// <summary>
		/// Finalizer<br/>
		/// 析构函数<br/>
		/// </summary>
		~SimpleDisposable() {
			Dispose();
		}
	}
}
