using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.Collections {
	/// <summary>
	/// 延迟创建类
	/// 线程安全且支持重置对象
	/// </summary>
	/// <typeparam name="T">对象类型</typeparam>
	public class LazyCache<T> where T : class {
		/// <summary>
		/// 实例
		/// </summary>
		private T Instance { get; set; }
		/// <summary>
		/// 生成器
		/// </summary>
		private Func<T> Factory { get; set; }
		/// <summary>
		/// 线程锁
		/// </summary>
		private object Lock { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="factory">生成器</param>
		public LazyCache(Func<T> factory) {
			Instance = null;
			Factory = factory;
			Lock = new object();
		}

		/// <summary>
		/// 判断实例是否已生成
		/// </summary>
		public bool IsValueCreated {
			get { return Instance != null; }
		}

		/// <summary>
		/// 获取实例，没有生成时执行生成
		/// </summary>
		public T Value {
			get {
				// 先保存本地对象，防止其他线程调用Reset
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
		/// 重置实例，下次获取时重新生成
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
	/// 延迟创建类的帮助函数
	/// </summary>
	public static class LazyCache {
		/// <summary>
		/// 创建延迟创建对象
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="factory">生成器</param>
		/// <returns></returns>
		public static LazyCache<T> Create<T>(Func<T> factory) where T : class {
			return new LazyCache<T>(factory);
		}
	}
}
