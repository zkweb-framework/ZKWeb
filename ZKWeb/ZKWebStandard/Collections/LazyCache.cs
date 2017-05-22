using System;

namespace ZKWebStandard.Collections {
	/// <summary>
	/// Lazy single object cache<br/>
	/// Support reset object and it's thread safe<br/>
	/// 单个值的懒缓存<br/>
	/// 支持重置对象并且它是线程安全的<br/>
	/// </summary>
	/// <typeparam name="T">Object type</typeparam>
	/// <example>
	/// <code language="cs">
	/// var cache = LazyCache.Create(() =&gt; { Console.WriteLine("create value"); return 123; });
	/// var value = cache.Value;
	/// cache.Reset();
	/// var value = cache.Value;
	/// </code>
	/// </example>
	/// <seealso cref="LazyCache"/>
	public class LazyCache<T> where T : class {
		/// <summary>
		/// Object instance<br/>
		/// 对象的实例<br/>
		/// </summary>
		private T Instance { get; set; }
		/// <summary>
		/// Object factory<br/>
		/// 对象的生成函数<br/>
		/// </summary>
		private Func<T> Factory { get; set; }
		/// <summary>
		/// Thread lock<br/>
		/// 线程锁<br/>
		/// </summary>
		private object Lock { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="factory">Object factory</param>
		public LazyCache(Func<T> factory) {
			Instance = null;
			Factory = factory;
			Lock = new object();
		}

		/// <summary>
		/// Determine object instance is created<br/>
		/// 检测对象是否已创建<br/>
		/// </summary>
		public bool IsValueCreated {
			get { return Instance != null; }
		}

		/// <summary>
		/// Get object instance, create it first if it's not created before<br/>
		/// 获取对象实例, 如果对象未创建则创建它<br/>
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
		/// Instance will be create again at the next time call "Value"<br/>
		/// 重置对象实例<br/>
		/// 下次调用"Value"属性时实例会被重新创建<br/>
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
	/// 懒缓存的工具函数<br/>
	/// </summary>
	/// <seealso cref="LazyCache{T}"/>
	public static class LazyCache {
		/// <summary>
		/// Create a lazy cached object<br/>
		/// 创建懒缓存对象<br/>
		/// </summary>
		/// <typeparam name="T">Object type</typeparam>
		/// <param name="factory">Object factory</param>
		/// <returns></returns>
		public static LazyCache<T> Create<T>(Func<T> factory) where T : class {
			return new LazyCache<T>(factory);
		}
	}
}
