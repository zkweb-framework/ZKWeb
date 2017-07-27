using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.FastReflection;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using ZKWebStandard.Ioc;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// IContainer extension methods<br/>
	/// 容器的扩展函数<br/>
	/// </summary>
	public static class IContainerExtensions {
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
		public static Func<object> BuildFactory(
			this IContainer container, Func<object> originalFactory, ReuseType reuseType) {
			if (reuseType == ReuseType.Transient) {
				// Transient
				return originalFactory;
			} else if (reuseType == ReuseType.Singleton) {
				// Singleton
				object value = null;
				object valueLock = new object();
				return () => {
					if (value != null) {
						return value;
					}
					lock (valueLock) {
						if (value != null) {
							return value; // double check
						}
						value = originalFactory();
						return value;
					}
				};
			} else if (reuseType == ReuseType.Scoped) {
				// Scoped, no need to lock
				var local = new AsyncLocal<object>();
				return () => {
					var value = local.Value;
					if (value != null) {
						return value;
					}
					value = originalFactory();
					local.Value = value;
					if (value is IDisposable disposable) {
						container.DisposeWhenScopeFinished(disposable);
					}
					return value;
				};
			} else {
				throw new NotSupportedException(string.Format("unsupported reuse type {0}", reuseType));
			}
		}

		/// <summary>
		/// Cache for type and it's original factory<br/>
		/// 缓存类型和它的原始工厂函数<br/>
		/// </summary>
		private readonly static ConcurrentDictionary<Type, Func<object>> TypeFactorysCache =
			new ConcurrentDictionary<Type, Func<object>>();

		/// <summary>
		/// Build factory from type and reuse type<br/>
		/// 根据类型和重用类型构建工厂函数<br/>
		/// </summary>
		/// <param name="container">IoC container</param>
		/// <param name="type">The type</param>
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
		/// var factory = container.BuildFactory(typeof(TestInjection), ReuseType.Transient);
		/// var testInjection = (TestInjection)factory();
		/// </code>
		/// </example>
		public static Func<object> BuildFactory(
			this IContainer container, Type type, ReuseType reuseType) {
			var typeFactor = TypeFactorysCache.GetOrAdd(type, t => {
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
					var parameterTypeInfo = parameterType.GetTypeInfo();
					if (parameterTypeInfo.IsGenericType &&
						parameterTypeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
						// paramerer is IEnumeable<T>
						argumentExpressions.Add(Expression.Call(
							Expression.Constant(container), nameof(IContainer.ResolveMany),
							parameterTypeInfo.GetGenericArguments(),
							Expression.Constant(null)));
					} else if (!parameter.HasDefaultValue) {
						// parameter didn't have a default value, use generic resolve
						argumentExpressions.Add(Expression.Call(
							Expression.Constant(container), nameof(IContainer.Resolve),
							new[] { parameterType },
							Expression.Constant(IfUnresolved.ReturnDefault),
							Expression.Constant(null)));
					} else {
						// parameter have a default value, use non generic resolve
						argumentExpressions.Add(
							Expression.Convert(
								Expression.Coalesce(
									Expression.Call(
										Expression.Constant(container), nameof(IContainer.Resolve),
										new Type[] { },
										Expression.Constant(parameterType),
										Expression.Constant(IfUnresolved.ReturnDefault),
										Expression.Constant(null)),
									Expression.Convert(Expression.Constant(parameter.DefaultValue), typeof(object))),
								parameterType));
					}
				}
				var newExpression = Expression.New(constructor, argumentExpressions);
				return Expression.Lambda<Func<object>>(newExpression).Compile();
			});
			return container.BuildFactory(typeFactor, reuseType);
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
				container.RegisterInstance<IServiceProvider>(provider);
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
				// Build factory
				Func<object> factory;
				if (descriptor.ImplementationType != null) {
					factory = container.BuildFactory(descriptor.ImplementationType, reuseType);
				} else if (descriptor.ImplementationFactory != null) {
					factory = container.BuildFactory(() => descriptor.ImplementationFactory(provider), reuseType);
				} else {
					factory = () => descriptor.ImplementationInstance;
				}
				// Register to container, don't wrap the factory again
				container.RegisterDelegate(descriptor.ServiceType, factory, ReuseType.Transient, null);
			}
		}

		/// <summary>
		/// IContainer => IServiceProvider Adapter
		/// </summary>
		private class ServiceProviderAdapter : IServiceProvider {
			public IContainer Container { get; set; }
			public MethodInfo ListFactoryMethod { get; set; }
			public MethodInfo IEnumerableFactoryMethod { get; set; }
			public MethodInfo LazyFactoryMethod { get; set; }

			public ServiceProviderAdapter(IContainer container) {
				Container = container;
				ListFactoryMethod = GetType().GetMethod(nameof(ListFactory));
				IEnumerableFactoryMethod = GetType().GetMethod(nameof(IEnumerableFactory));
				LazyFactoryMethod = GetType().GetMethod(nameof(LazyFactory));
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
				// Detect wrapper
				var isLazy = false;
				var isIList = false;
				var isIEnumerable = false;
				if (serviceType.IsGenericType) {
					// Detect Lazy<T>
					if (serviceType.GetGenericTypeDefinition() == typeof(Lazy<>)) {
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
				}
				// Resolve implementation
				var resolve = new Func<object>(() => {
					if (isIList) {
						return ListFactoryMethod.MakeGenericMethod(serviceType).FastInvoke(this);
					} else if (isIEnumerable) {
						return IEnumerableFactoryMethod.MakeGenericMethod(serviceType).FastInvoke(this);
					}
					return Container.Resolve(serviceType, IfUnresolved.ReturnDefault);
				});
				if (isLazy) {
					return LazyFactoryMethod.MakeGenericMethod(serviceType).FastInvoke(this, resolve);
				}
				return resolve();
			}
		}
	}
}
