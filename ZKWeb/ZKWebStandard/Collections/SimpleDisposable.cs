using System;
using System.Threading;

namespace ZKWebStandard.Collections {
	/// <summary>
	/// Simple dispose object
	/// Execute the given method when disposed,
	/// the given method execute not more than once
	/// </summary>
	public class SimpleDisposable : IDisposable {
		/// <summary>
		/// Method execute when dispoed
		/// </summary>
		protected Action OnDispose { get; set; }
		/// <summary>
		/// Is method executed
		/// </summary>
		protected int Disposed = 0;

		/// <summary>
		/// Initialized
		/// </summary>
		public SimpleDisposable(Action onDispose) {
			OnDispose = onDispose;
		}

		/// <summary>
		/// Call the method if it's not called before
		/// </summary>
		public void Dispose() {
			if (Interlocked.Exchange(ref Disposed, 1) == 0) {
				OnDispose();
			}
		}

		/// <summary>
		/// Finalizer
		/// </summary>
		~SimpleDisposable() {
			Dispose();
		}
	}
}
