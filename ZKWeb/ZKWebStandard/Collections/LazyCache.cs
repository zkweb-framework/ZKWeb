using System;

namespace ZKWebStandard.Collections {
	/// <summary>
	/// Lazy single object cache<br/>
	/// Support reset object and it's thread safe<br/>
	/// <br/>
	/// <br/>
	/// </summary>
	/// <typeparam name="T">Object type</typeparam>
	public class LazyCache<T> where T : class {
		/// <summary>
		/// Object instance<br/>
		/// <br/>
		/// </summary>
		private T Instance { get; set; }
		/// <summary>
		/// Object factory<br/>
		/// <br/>
		/// </summary>
		private Func<T> Factory { get; set; }
		/// <summary>
		/// Thread lock<br/>
		/// <br/>
		/// </summary>
		private object Lock { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		/// <param name="factory">Object factory</param>
		public LazyCache(Func<T> factory) {
			Instance = null;
			Factory = factory;
			Lock = new object();
		}

		/// <summary>
		/// Determine object instance is created<br/>
		/// <br/>
		/// </summary>
		public bool IsValueCreated {
			get { return Instance != null; }
		}

		/// <summary>
		/// Get object instance, create it first if it's not created before<br/>
		/// <br/>
		/// </summary>
		public T Value {
			get {
				// Copy to local object
				// It will not affect by other threads that may call `Reset`
				var inst = Instance;
				if (inst == null) {
					lock (Lock) {
						if (Instance == null) {
							Instance = Factory();
						}
						inst = Instance;
					}
				}
				return inst;
			}
		}

		/// <summary>
		/// Reset object instance<br/>
		/// Instance will be create again at the next time call `Value`<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		public void Reset() {
			if (Instance != null) {
				lock (Lock) {
					Instance = null;
				}
			}
		}
	}

	/// <summary>
	/// LazyCache utility functions<br/>
	/// <br/>
	/// </summary>
	public static class LazyCache {
		/// <summary>
		/// Create a lazy cached object<br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="T">Object type</typeparam>
		/// <param name="factory">Object factory</param>
		/// <returns></returns>
		public static LazyCache<T> Create<T>(Func<T> factory) where T : class {
			return new LazyCache<T>(factory);
		}
	}
}
