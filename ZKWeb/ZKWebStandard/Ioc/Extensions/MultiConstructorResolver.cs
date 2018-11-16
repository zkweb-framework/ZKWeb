using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ZKWebStandard.Ioc.Extensions {
	/// <summary>
	/// Multi constructor resolver<br/>
	/// 多构造函数解决器<br/>
	/// </summary>
	public class MultiConstructorResolver : IMultiConstructorResolver {
		/// <summary>
		/// Container<br/>
		/// 容器<br/>
		/// </summary>
		private IContainer _container { get; set; }
		/// <summary>
		/// Factory cache, it should be per container<br/>
		/// 工厂函数的缓存, 它应该是跟随容器的<br/>
		/// </summary>
		private ConcurrentDictionary<Type, ContainerFactoryDelegate> _factoryCache { get; set; }

		/// <summary>
		/// Initliaze<br/>
		/// 初始化<br/>
		/// </summary>
		public MultiConstructorResolver(IContainer container) {
			_container = container;
			_factoryCache = new ConcurrentDictionary<Type, ContainerFactoryDelegate>();
		}

		/// <summary>
		/// Resolve type have multi constructors<br/>
		/// 解决拥有多个构造函数的类型<br/>
		/// </summary>
		public object Resolve(Type type) {
			object resolved = null;
			var resolvedSet = false;
			var factory = _factoryCache.GetOrAdd(type, _ => {
				// First, build factories for all constructors
				var constructors = type.GetConstructors()
					.Where(c => c.IsPublic)
					.OrderByDescending(c => c.GetParameters().Length)
					.ToList();
				foreach (var constructor in constructors) {
					// Then, try pick one that successfully resolved
					try {
						var innerFactory = ContainerFactoryBuilder.BuildFactory(constructor);
						resolved = innerFactory(_container, type);
						resolvedSet = true;
						// Finally, remember which factory is used
						return innerFactory;
					} catch {
						// Retry next constructor
					}
				}
				return null;
			});
			if (factory == null) {
				throw new KeyNotFoundException($"No factories are able to resolve type {type}");
			}
			if (resolvedSet) {
				return resolved;
			}
			return factory(_container, type);
		}

		/// <summary>
		/// Clear factory cache, please run it after container modified<br/>
		/// 清空工厂函数的缓存, 请在修改容器后运行它<br/>
		/// </summary>
		public void ClearCache() {
			_factoryCache.Clear();
		}
	}
}
