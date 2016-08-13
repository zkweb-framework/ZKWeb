using System;

namespace ZKWebStandard.Collections {
	/// <summary>
	/// Lazy cache, support reset object and thread safe
	/// </summary>
	/// <typeparam name="T">Object type</typeparam>
	public class LazyCache<T> where T : class {
		/// <summary>
		/// Object instance
		/// </summary>
		private T Instance { get; set; }
		/// <summary>
		/// Object factory
		/// </summary>
		private Func<T> Factory { get; set; }
		/// <summary>
		/// Thread lock
		/// </summary>
		private object Lock { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="factory">Object factory</param>
		public LazyCache(Func<T> factory) {
			Instance = null;
			Factory = factory;
			Lock = new object();
		}

		/// <summary>
		/// Determine object instance is created
		/// </summary>
		public bool IsValueCreated {
			get { return Instance != null; }
		}

		/// <summary>
		/// Get object instance, create it first if it's not created before
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
		/// Reset object instance
		/// Instance will be create again at the next time call `Value`
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
	/// LazyCache utility functions
	/// </summary>
	public static class LazyCache {
		/// <summary>
		/// Create a lazy cached object
		/// </summary>
		/// <typeparam name="T">Object type</typeparam>
		/// <param name="factory">Object factory</param>
		/// <returns></returns>
		public static LazyCache<T> Create<T>(Func<T> factory) where T : class {
			return new LazyCache<T>(factory);
		}
	}
}
