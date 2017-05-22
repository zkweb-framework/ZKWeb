using System;
using System.Threading;

namespace ZKWebStandard.Collections {
	/// <summary>
	/// Simple dispose object<br/>
	/// Execute the given method when disposed,<br/>
	/// the given method execute not more than once<br/>
	/// <br/>
	/// <br/>
	/// <br/>
	/// </summary>
	public class SimpleDisposable : IDisposable {
		/// <summary>
		/// Method execute when dispoed<br/>
		/// <br/>
		/// </summary>
		protected Action OnDispose { get; set; }
		/// <summary>
		/// Is method executed<br/>
		/// <br/>
		/// </summary>
		protected int Disposed = 0;

		/// <summary>
		/// Initialized<br/>
		/// <br/>
		/// </summary>
		public SimpleDisposable(Action onDispose) {
			OnDispose = onDispose;
		}

		/// <summary>
		/// Call the method if it's not called before<br/>
		/// <br/>
		/// </summary>
		public void Dispose() {
			if (Interlocked.Exchange(ref Disposed, 1) == 0) {
				OnDispose();
			}
		}

		/// <summary>
		/// Finalizer<br/>
		/// <br/>
		/// </summary>
		~SimpleDisposable() {
			Dispose();
		}
	}
}
