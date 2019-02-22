using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Factory data<br/>
	/// 工厂函数数据<br/>
	/// </summary>
	/// <seealso cref="Container"/>
	public class ContainerFactoryData {
		/// <summary>
		/// Factory function<br/>
		/// 工厂函数<br/>
		/// </summary>
		private ContainerFactoryDelegate _factoryFunc { get; set; }
		/// <summary>
		/// Lock for singleton or scoped factory<br/>
		/// 用于创建单例或区域实例的线程锁<br/>
		/// </summary>
		private object _factoryLock { get; set; }
		/// <summary>
		/// Is implementation type belong generic definition<br/>
		/// 实现类型是否属于泛型定义<br/>
		/// </summary>
		private bool _isGenericTypeDefinition { get; set; }
		/// <summary>
		/// The instance object for singleton reuse<br/>
		/// 单例重用时使用的实例对象<br/>
		/// </summary>
		private object _singletonInstance { get; set; }
		/// <summary>
		/// The instance object for scoped reuse<br/>
		/// 区域重用时使用的实例对象<br/>
		/// </summary>
		private AsyncLocal<object> _scopedInstance { get; set; }
		/// <summary>
		/// Reuse type<br/>
		/// 重用类型<br/>
		/// </summary>
		public ReuseType ReuseType { get; private set; }
		/// <summary>
		/// Implementation type hint<br/>
		/// Usually it's the type factory function will return<br/>
		/// 实现类型<br/>
		/// 通常它会是工厂函数返回的对象的类型<br/>
		/// </summary>
		public Type ImplementationTypeHint { get; private set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public ContainerFactoryData(
			ContainerFactoryDelegate factoryFunc,
			ReuseType reuseType,
			Type implementationTypeHint) {
			_factoryFunc = factoryFunc;
			_factoryLock = new object();
			_isGenericTypeDefinition = implementationTypeHint.IsGenericTypeDefinition;
			if (reuseType == ReuseType.Singleton) {
				if (_isGenericTypeDefinition) {
					_singletonInstance = new ConcurrentDictionary<string, object>();
				}
			} else if (reuseType == ReuseType.Scoped) {
				_scopedInstance = new AsyncLocal<object>();
			} else if (reuseType != ReuseType.Transient) {
				throw new NotSupportedException(string.Format(
					"Unsupported reuse type {0}", ReuseType));
			}
			ReuseType = reuseType;
			ImplementationTypeHint = implementationTypeHint;
		}

		/// <summary>
		/// Get instance<br/>
		/// 获取实例<br/>
		/// </summary>
		public object GetInstance(IContainer container, Type serviceType) {
			if (ReuseType == ReuseType.Transient) {
				// Transient reuse
				return _factoryFunc(container, serviceType);
			} else if (ReuseType == ReuseType.Singleton) {
				if (!_isGenericTypeDefinition) {
					// Singleton reuse, non generic
					if (_singletonInstance != null) {
						return _singletonInstance;
					}
					lock (_factoryLock) {
						if (_singletonInstance != null) {
							return _singletonInstance;
						}
						var instance = _factoryFunc(container, serviceType);
						Interlocked.MemoryBarrier();
						_singletonInstance = instance;
						return instance;
					}
				} else {
					// Singleton reuse, generic
					var dict = (ConcurrentDictionary<string, object>)_singletonInstance;
					var key = string.Join("\r\n", serviceType.GetGenericArguments().Select(t => t.FullName));
					if (dict.TryGetValue(key, out var instance)) {
						return instance;
					}
					lock (_factoryLock) {
						if (dict.TryGetValue(key, out instance)) {
							return instance;
						}
						instance = _factoryFunc(container, serviceType);
						Interlocked.MemoryBarrier();
						dict[key] = instance;
						return instance;
					}
				}
			} else if (ReuseType == ReuseType.Scoped) {
				if (!_isGenericTypeDefinition) {
					// Scoped reuse, non generic
					var instance = _scopedInstance.Value;
					if (instance != null) {
						return instance;
					}
					lock (_factoryLock) {
						instance = _scopedInstance.Value;
						if (instance != null) {
							return instance;
						}
						instance = _factoryFunc(container, serviceType);
						Interlocked.MemoryBarrier();
						_scopedInstance.Value = instance;
						if (instance is IDisposable disposable) {
							container.DisposeWhenScopeFinished(disposable);
						}
						return instance;
					}
				} else {
					// Scoped reuse, generic
					var dict = (ConcurrentDictionary<string, object>)_scopedInstance.Value;
					if (dict == null) {
						lock (_factoryLock) {
							dict = (ConcurrentDictionary<string, object>)_scopedInstance.Value;
							if (dict == null) {
								dict = new ConcurrentDictionary<string, object>();
								Interlocked.MemoryBarrier();
								_scopedInstance.Value = dict;
							}
						}
					}
					var key = string.Join("\r\n", serviceType.GetGenericArguments().Select(t => t.FullName));
					if (dict.TryGetValue(key, out var instance)) {
						return instance;
					}
					lock (_factoryLock) {
						if (dict.TryGetValue(key, out instance)) {
							return instance;
						}
						instance = _factoryFunc(container, serviceType);
						Interlocked.MemoryBarrier();
						dict[key] = instance;
						if (instance is IDisposable disposable) {
							container.DisposeWhenScopeFinished(disposable);
						}
						return instance;
					}
				}
			} else {
				throw new NotSupportedException(string.Format(
					"Unsupported reuse type {0}", ReuseType));
			}
		}
	}
}
