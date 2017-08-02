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
		protected IContainer Container { get; set; }
		/// <summary>
		/// Factory cache, it should be per container<br/>
		/// 工厂函数的缓存, 它应该是跟随容器的<br/>
		/// </summary>
		protected ConcurrentDictionary<Type, object> FactoryCache { get; set; }

		/// <summary>
		/// Initliaze<br/>
		/// 初始化<br/>
		/// </summary>
		public MultiConstructorResolver(IContainer container) {
			Container = container;
			FactoryCache = new ConcurrentDictionary<Type, object>();
		}

		/// <summary>
		/// Resolve type have multi constructors<br/>
		/// 解决拥有多个构造函数的类型<br/>
		/// </summary>
		public T Resolve<T>() {
			T resolved = default(T);
			var resolvedSet = false;
			var factory = FactoryCache.GetOrAdd(typeof(T), type => {
				// First, build factories for all constructors
				var constructors = type.GetConstructors()
					.Where(c => c.IsPublic)
					.OrderByDescending(c => c.GetParameters().Length)
					.ToList();
				foreach (var constructor in constructors) {
					// Then, try pick one that successfully resolved
					try {
						var innerFactory = Container.GenericBuildFactoryFromConstructor<T>(constructor);
						resolved = innerFactory();
						resolvedSet = true;
						// Finally, remember which factory is used
						return innerFactory;
					} catch {
					}
				}
				return null;
			});
			if (factory == null) {
				throw new KeyNotFoundException($"no factories are able to resolve type {typeof(T)}");
			}
			if (resolvedSet) {
				return resolved;
			}
			return ((Func<T>)factory)();
		}

		/// <summary>
		/// Clear factory cache, please run it after alter container<br/>
		/// 清空工厂函数的缓存, 请在修改容器后运行它<br/>
		/// </summary>
		public void ClearCache() {
			FactoryCache.Clear();
		}
	}
}
