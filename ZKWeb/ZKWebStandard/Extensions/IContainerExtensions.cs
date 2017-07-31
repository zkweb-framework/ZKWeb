using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.FastReflection;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using ZKWebStandard.Collections;
using ZKWebStandard.Ioc;
using ZKWebStandard.Utils;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// IContainer extension methods<br/>
	/// 容器的扩展函数<br/>
	/// </summary>
	public static class IContainerExtensions {
		private readonly static ConcurrentDictionary<Type, object> TypeFactoriesCache =
			new ConcurrentDictionary<Type, object>();
		private readonly static MethodInfo BuildFactoryT_3 = typeof(IContainerExtensions)
			.FastGetMethods().First(f => f.Name == nameof(BuildFactory) && f.IsGenericMethod && f.GetParameters().Length == 3);
		private readonly static MethodInfo BuildFactoryT_2 = typeof(IContainerExtensions)
			.FastGetMethods().First(f => f.Name == nameof(BuildFactory) && f.IsGenericMethod && f.GetParameters().Length == 2);
		private readonly static MethodInfo ToGenericFactory = typeof(IContainerExtensions)
			.FastGetMethod(nameof(ToGenericFactoryImpl), BindingFlags.NonPublic | BindingFlags.Static);
		private readonly static MethodInfo ToObjectFactory = typeof(IContainerExtensions)
			.FastGetMethod(nameof(ToObjectFactoryImpl), BindingFlags.NonPublic | BindingFlags.Static);
		private static Func<T> ToGenericFactoryImpl<T>(Func<object> objectFactory) => () => (T)objectFactory();
		private static Func<object> ToObjectFactoryImpl<T>(Func<T> genericFactory) => () => genericFactory();

		/// <summary>
		/// Build factory from original factory and reuse type<br/>
		/// 根据原工厂函数和重用类型构建新的工厂函数<br/>
		/// </summary>
		/// <param name="container">IoC container</param>
		/// <param name="originalFactory">Original factory</param>
		/// <param name="reuseType">Reuse type</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// class TestData { }
		/// 
		/// var container = new Container();
		/// var factoryA = container.BuildFactory(() =&gt; new TestData(), ReuseType.Transient);
		/// // !object.ReferenceEquals(factoryA(), factoryA())
		/// var factoryB = container.BuildFactory(() =&gt; new TestData(), ReuseType.Singleton);
		/// // object.ReferenceEquals(factoryB(), factoryB())
		/// </code>
		/// </example>
		public static Func<T> BuildFactory<T>(
			this IContainer container, Func<T> originalFactory, ReuseType reuseType) {
			if (reuseType == ReuseType.Transient) {
				// Transient
				return originalFactory;
			} else if (reuseType == ReuseType.Singleton) {
				// Singleton
				var value = default(T);
				var valueCreated = false;
				var valueLock = new object();
				return () => {
					if (valueCreated) {
						return value;
					}
					lock (valueLock) {
						if (valueCreated) {
							return value; // double check
						}
						value = originalFactory();
						Interlocked.MemoryBarrier();
						valueCreated = true;
						return value;
					}
				};
			} else if (reuseType == ReuseType.Scoped) {
				// Scoped
				var value = new AsyncLocal<Pair<bool, T>>();
				var valueLock = new object();
				return () => {
					var pair = value.Value;
					if (pair.First) {
						return pair.Second;
					}
					lock (valueLock) {
						pair = value.Value;
						if (pair.First) {
							return pair.Second; // double check
						}
						pair = Pair.Create(true, originalFactory());
						if (pair.Second is IDisposable disposable) {
							container.DisposeWhenScopeFinished(disposable);
						}
						value.Value = pair;
						return pair.Second;
					}
				};
			} else {
				throw new NotSupportedException(string.Format("unsupported reuse type {0}", reuseType));
			}
		}

		/// <summary>
		/// Build factory from type and reuse type<br/>
		/// 根据类型和重用类型构建工厂函数<br/>
		/// </summary>
		/// <typeparam name="T">The type</typeparam>
		/// <param name="container">IoC container</param>
		/// <param name="reuseType">Reuse type</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// class TestData { }
		/// 
		/// class TestInjection {
		/// 	public TestData Data { get; set; }
		/// 	public TestInjection(TestData data) { Data = data; }
		/// }
		/// 
		/// IContainer container = new Container();
		/// container.RegisterMany&lt;TestData&gt;();
		/// var factory = container.BuildFactory&gt;TestInjection&lt;(ReuseType.Transient);
		/// var testInjection = factory();
		/// </code>
		/// </example>
		public static Func<T> BuildFactory<T>(this IContainer container, ReuseType reuseType) {
			var type = typeof(T);
			var typeFactory = TypeFactoriesCache.GetOrAdd(type, t => {
				// Support constructor dependency injection
				// First detect Inject attribute, then use the constructor that have most parameters	
				var argumentExpressions = new List<Expression>();
				var constructors = t.GetConstructors();
				var constructor = constructors.FirstOrDefault(
					c => c.GetAttribute<InjectAttribute>() != null);
				if (constructor == null) {
					constructor = constructors
						.Where(c => c.IsPublic)
						.OrderByDescending(c => c.GetParameters().Length)
						.FirstOrDefault();
				}
				if (constructor == null) {
					throw new ArgumentException(
						$"Type {type} should have atleast one public constructor");
				}
				foreach (var parameter in constructor.GetParameters()) {
					var parameterType = parameter.ParameterType;
					var isFunc = false;
					var isLazy = false;
					var isIList = false;
					var isIEnumerable = false;
					// Detect Func<T>
					if (parameterType.IsGenericType &&
						parameterType.GetGenericTypeDefinition() == typeof(Func<>)) {
						isFunc = true;
						parameterType = parameterType.GetGenericArguments()[0];
					}
					// Detect Lazy<T>
					if (parameterType.IsGenericType &&
						parameterType.GetGenericTypeDefinition() == typeof(Lazy<>)) {
						isLazy = true;
						parameterType = parameterType.GetGenericArguments()[0];
					}
					// Detect IList<T> and IEnumerable<T>
					if (!parameterType.IsGenericType) {
					} else if (parameterType.GetGenericTypeDefinition() == typeof(List<>) ||
						parameterType.GetGenericTypeDefinition() == typeof(IList<>) ||
						parameterType.GetGenericTypeDefinition() == typeof(ICollection<>)) {
						isIList = true;
						parameterType = parameterType.GetGenericArguments()[0];
					} else if (parameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
						isIEnumerable = true;
						parameterType = parameterType.GetGenericArguments()[0];
					}
					// Build resolve expression
					Expression argumentExpression;
					if (isIList || isIEnumerable) {
						// Use ResolveMany<T>
						argumentExpression = Expression.Call(
							Expression.Constant(container), nameof(IContainer.ResolveMany),
							new[] { parameterType },
							Expression.Constant(null));
						if (isIList) {
							argumentExpression = Expression.Call(
								typeof(Enumerable),
								nameof(Enumerable.ToList),
								new Type[] { parameterType },
								argumentExpression);
						}
					} else if (!parameter.HasDefaultValue) {
						// Use Resolve<T>
						argumentExpression = Expression.Call(
							Expression.Constant(container), nameof(IContainer.Resolve),
							new[] { parameterType },
							Expression.Constant(IfUnresolved.ReturnDefault),
							Expression.Constant(null));
					} else {
						// Use generic Resolve with ?? operator
						argumentExpression = Expression.Convert(
							Expression.Coalesce(
								Expression.Call(
									Expression.Constant(container), nameof(IContainer.Resolve),
									new Type[0],
									Expression.Constant(parameterType),
									Expression.Constant(IfUnresolved.ReturnDefault),
									Expression.Constant(null)),
								Expression.Convert(Expression.Constant(parameter.DefaultValue), typeof(object))),
							parameterType);
					}
					if (isLazy) {
						// Wrap with Lazy<T>
						var argumentFactoryType = typeof(Func<>).MakeGenericType(argumentExpression.Type);
						var argumentFactory = Expression.Lambda(argumentFactoryType, argumentExpression).Compile();
						var lazyConstructor = typeof(Lazy<>).MakeGenericType(argumentExpression.Type).GetConstructors()
							.First(c => c.GetParameters().Length == 1 &&
								c.GetParameters()[0].ParameterType == argumentFactoryType);
						argumentExpression = Expression.New(lazyConstructor, Expression.Constant(argumentFactory));
					}
					if (isFunc) {
						// Pass factory
						var argumentFactoryType = typeof(Func<>).MakeGenericType(argumentExpression.Type);
						var argumentFactory = Expression.Lambda(argumentFactoryType, argumentExpression).Compile();
						argumentExpression = Expression.Constant(argumentFactory);
					}
					argumentExpressions.Add(argumentExpression);
				}
				var newExpression = Expression.New(constructor, argumentExpressions);
				return Expression.Lambda<Func<T>>(newExpression).Compile();
			});
			return container.BuildFactory((Func<T>)typeFactory, reuseType);
		}

		/// <summary>
		/// Build factory from original factory and reuse type<br/>
		/// Return genericFactory for Func&lt;T&gt; and objectFactory for Func&lt;object&gt;<br/>
		/// 根据原工厂函数和重用类型构建新的工厂函数<br/>
		/// 返回Func&lt;T&gt;类型的genericFactory和Func&lt;object&gt;类型的objectFactory<br/>
		/// </summary>
		/// <seealso cref="BuildFactory{T}(IContainer, Func{T}, ReuseType)"/>
		public static void BuildFactory(
			this IContainer container, Type type, Func<object> originalFactory, ReuseType reuseType,
			out object genericFactory, out Func<object> objectFactory) {
			if (type.IsGenericTypeDefinition) {
				throw new NotSupportedException("Register generic definition with factory is unsupported");
			}
			var invoker = ReflectionUtils.MakeInvoker(BuildFactoryT_3, typeof(object));
			objectFactory = (Func<object>)invoker(null, new object[] { container, originalFactory, reuseType });
			genericFactory = ReflectionUtils.MakeInvoker(ToGenericFactory, type)(null, new object[] { objectFactory });
		}

		/// <summary>
		/// Build factory from type and reuse type<br/>
		/// Return genericFactory for Func&lt;T&gt; and objectFactory for Func&lt;object&gt;<br/>
		/// 根据类型和重用类型构建工厂函数<br/>
		/// 返回Func&lt;T&gt;类型的genericFactory和Func&lt;object&gt;类型的objectFactory<br/>
		/// </summary>
		/// <seealso cref="BuildFactory{T}(IContainer, ReuseType)"/>
		public static void BuildFactory(
			this IContainer container, Type type, ReuseType reuseType,
			out object genericFactory, out Func<object> objectFactory) {
			if (type.IsGenericTypeDefinition) {
				// Register Implementation<T> to Service<T>
				var factoryCache = new ConcurrentDictionary<Type, Func<object>>();
				var factoryFactory = new Func<Func<Type, object>>(() => {
					return new Func<Type, object>(serviceType => {
						var bindType = type.MakeGenericType(serviceType.GetGenericArguments());
						var factory = factoryCache.GetOrAdd(serviceType, _ => {
							var invoker = ReflectionUtils.MakeInvoker(BuildFactoryT_2, bindType);
							var innerGenericFactory = invoker(null, new object[] { container, reuseType });
							var innerObjectFactory = (Func<object>)ReflectionUtils.MakeInvoker(
								ToObjectFactory, bindType)(null, new object[] { innerGenericFactory });
							return innerObjectFactory;
						});
						return factory();
					});
				});
				objectFactory = factoryFactory;
				genericFactory = objectFactory;
			} else {
				// Normal case
				var invoker = ReflectionUtils.MakeInvoker(BuildFactoryT_2, type);
				genericFactory = invoker(null, new object[] { container, reuseType });
				objectFactory = (Func<object>)ReflectionUtils.MakeInvoker(ToObjectFactory, type)(null, new object[] { genericFactory });
			}
		}

		/// <summary>
		/// Construct an IServiceProvider adapter<br/>
		/// 构建一个IServiceProvider转接器<br/>
		/// </summary>
		/// <param name="container">IoC container</param>
		/// <returns></returns>
		public static IServiceProvider AsServiceProvider(this IContainer container) {
			var provider = container.Resolve<IServiceProvider>(IfUnresolved.ReturnDefault);
			if (provider == null) {
				provider = new ServiceProviderAdapter(container);
				container.Unregister<IServiceProvider>();
				container.Unregister<IServiceScopeFactory>();
				container.RegisterInstance<IServiceProvider>(provider);
				container.RegisterInstance<IServiceScopeFactory>(new ServiceScopeFactory(container));
			}
			return provider;
		}

		/// <summary>
		/// Register services from IServiceCollection<br/>
		/// 从IServiceCollection注册服务<br/>
		/// </summary>
		/// <param name="container">IoC container</param>
		/// <param name="serviceCollection">Service collection</param>
		public static void RegisterFromServiceCollection(
			this IContainer container, IServiceCollection serviceCollection) {
			var provider = container.AsServiceProvider();
			foreach (var descriptor in serviceCollection) {
				// Convert ReuseType
				ReuseType reuseType;
				if (descriptor.Lifetime == ServiceLifetime.Transient) {
					reuseType = ReuseType.Transient;
				} else if (descriptor.Lifetime == ServiceLifetime.Singleton) {
					reuseType = ReuseType.Singleton;
				} else if (descriptor.Lifetime == ServiceLifetime.Scoped) {
					reuseType = ReuseType.Scoped;
				} else {
					throw new NotSupportedException($"Unsupported ServiceLifetime: {descriptor.Lifetime}");
				}
				// Register service
				if (descriptor.ImplementationType != null) {
					container.Register(descriptor.ServiceType,
						descriptor.ImplementationType, reuseType, null);
				} else if (descriptor.ImplementationFactory != null) {
					container.RegisterDelegate(descriptor.ServiceType,
						() => descriptor.ImplementationFactory(provider), reuseType, null);
				} else {
					container.RegisterInstance(
						descriptor.ServiceType, descriptor.ImplementationInstance, null);
				}
			}
		}

		/// <summary>
		/// IContainer => IServiceProvider Adapter<br/>
		/// 转换IContainer到IServiceProvider的接口<br/>
		/// </summary>
		private class ServiceProviderAdapter : IServiceProvider {
			public IContainer Container { get; set; }
			public MethodInfo ListFactoryMethod { get; set; }
			public MethodInfo IEnumerableFactoryMethod { get; set; }
			public MethodInfo LazyFactoryMethod { get; set; }

			public ServiceProviderAdapter(IContainer container) {
				Container = container;
				ListFactoryMethod = GetType().FastGetMethod(nameof(ListFactory));
				IEnumerableFactoryMethod = GetType().FastGetMethod(nameof(IEnumerableFactory));
				LazyFactoryMethod = GetType().FastGetMethod(nameof(LazyFactory));
			}

			public List<T> ListFactory<T>() {
				return Container.ResolveMany<T>().ToList();
			}

			public IEnumerable<T> IEnumerableFactory<T>() {
				return Container.ResolveMany<T>();
			}

			public Lazy<T> LazyFactory<T>(Func<object> resolve) {
				return new Lazy<T>(() => (T)resolve());
			}

			public object GetService(Type serviceType) {
				var isFunc = false;
				var isLazy = false;
				var isIList = false;
				var isIEnumerable = false;
				// Detect Func<T>
				Type funcReturnType = null;
				if (serviceType.IsGenericType &&
					serviceType.GetGenericTypeDefinition() == typeof(Func<>)) {
					isFunc = true;
					serviceType = funcReturnType = serviceType.GetGenericArguments()[0];
				}
				// Detect Lazy<T>
				if (serviceType.IsGenericType &&
					serviceType.GetGenericTypeDefinition() == typeof(Lazy<>)) {
					isLazy = true;
					serviceType = serviceType.GetGenericArguments()[0];
				}
				// Detect IList<T> and IEnumerable<T>
				if (!serviceType.IsGenericType) {
				} else if (serviceType.GetGenericTypeDefinition() == typeof(List<>) ||
					serviceType.GetGenericTypeDefinition() == typeof(IList<>) ||
					serviceType.GetGenericTypeDefinition() == typeof(ICollection<>)) {
					isIList = true;
					serviceType = serviceType.GetGenericArguments()[0];
				} else if (serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
					isIEnumerable = true;
					serviceType = serviceType.GetGenericArguments()[0];
				}
				// Resolve implementation
				var resolve = new Func<object>(() => {
					if (isIList) {
						return ReflectionUtils.MakeInvoker(ListFactoryMethod, serviceType)(this, null);
					} else if (isIEnumerable) {
						return ReflectionUtils.MakeInvoker(IEnumerableFactoryMethod, serviceType)(this, null);
					}
					return Container.Resolve(serviceType, IfUnresolved.ReturnDefault);
				});
				if (isLazy) {
					var originalResolve = resolve;
					var invoker = ReflectionUtils.MakeInvoker(LazyFactoryMethod, serviceType);
					resolve = () => invoker(this, new object[] { originalResolve });
				}
				if (isFunc) {
					var originalResolve = resolve;
					var invoker = ReflectionUtils.MakeInvoker(ToGenericFactory, funcReturnType);
					resolve = () => invoker(null, new object[] { originalResolve });
				}
				return resolve();
			}
		}

		/// <summary>
		/// Service scope factory<br/>
		/// 范围的工厂类<br/>
		/// </summary>
		private class ServiceScopeFactory : IServiceScopeFactory {
			public IContainer Container { get; }

			public ServiceScopeFactory(IContainer container) {
				Container = container;
			}

			public IServiceScope CreateScope() {
				return new ServiceScope(Container);
			}

			private class ServiceScope : IServiceScope {
				public IContainer Container { get; }
				public IServiceProvider ServiceProvider { get; }

				public ServiceScope(IContainer container) {
					Container = container;
					ServiceProvider = container.AsServiceProvider();
				}

				public void Dispose() {
					Container.ScopeFinished();
				}
			}
		}
	}
}
