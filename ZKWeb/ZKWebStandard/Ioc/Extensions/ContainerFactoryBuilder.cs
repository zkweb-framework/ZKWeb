using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.FastReflection;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;

namespace ZKWebStandard.Ioc.Extensions {
	/// <summary>
	/// Provide methods build factory method from IContainer<br/>
	/// 提供从IContainer构建工厂函数的函数<br/>
	/// </summary>
	public static class ContainerFactoryBuilder {
		internal readonly static MethodInfo GenericWrapFactoryMethod =
			typeof(ContainerFactoryBuilder).FastGetMethod(nameof(GenericWrapFactory));
		internal readonly static MethodInfo GenericBuildAndWrapFactoryMethod =
			typeof(ContainerFactoryBuilder).FastGetMethod(nameof(GenericBuildAndWrapFactory));
		internal readonly static MethodInfo ToGenericFactoryMethod = typeof(ContainerFactoryBuilder)
			.FastGetMethod(nameof(ToGenericFactory), BindingFlags.NonPublic | BindingFlags.Static);
		internal readonly static MethodInfo ToObjectFactoryMethod = typeof(ContainerFactoryBuilder)
			.FastGetMethod(nameof(ToObjectFactory), BindingFlags.NonPublic | BindingFlags.Static);
		internal static Func<T> ToGenericFactory<T>(Func<object> objectFactory) => () => (T)objectFactory();
		internal static Func<object> ToObjectFactory<T>(Func<T> genericFactory) => () => genericFactory();

		/// <summary>
		/// Wrap factory with reuse type<br/>
		/// 根据重用类型包装工厂函数<br/>
		/// </summary>
		public static Func<T> GenericWrapFactory<T>(
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
		/// Build factory from constructor<br/>
		/// Please cache result from this method for better performance<br/>
		/// 根据构造函数构建工厂函数<br/>
		/// 请缓存这个函数的结果, 以得到更好的性能<br/>
		/// </summary>
		public static Func<T> GenericBuildFactoryFromConstructor<T>(
			this IContainer container, ConstructorInfo constructor) {
			var argumentExpressions = new List<Expression>();
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
						Expression.Constant(IfUnresolved.Throw),
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
		}

		/// <summary>
		/// Build factory from type and wrap it with reuse type<br/>
		/// Please cache result from this method for better performance<br/>
		/// 根据类型构建工厂函数, 并根据重用类型包装<br/>
		/// 请缓存这个函数的结果, 以得到更好的性能<br/>
		/// </summary>
		public static Func<T> GenericBuildAndWrapFactory<T>(
			this IContainer container, ReuseType reuseType) {
			var type = typeof(T);
			// Support constructor dependency injection
			// First: use constructor marked with InjectAttribute
			var constructors = type.GetConstructors();
			var constructor = constructors
				.Where(c => c.GetAttribute<InjectAttribute>() != null).SingleOrDefaultNotThrow();
			// Second: use the only public constructor
			if (constructor == null) {
				constructor = constructors.Where(c => c.IsPublic).SingleOrDefaultNotThrow();
			}
			// Third: use runtime resolver
			Func<T> factory;
			if (constructor != null) {
				factory = container.GenericBuildFactoryFromConstructor<T>(constructor);
			} else {
				factory = () => {
					var resolver = container.Resolve<IMultiConstructorResolver>(IfUnresolved.ReturnDefault);
					if (resolver == null) {
						throw new ArgumentException(
							$"Type {type} have multi constructor and no one is marked with [Inject]," +
							"please mark one with [Inject] or provide IMultiConstructorResolver");
					} else {
						return resolver.Resolve<T>();
					}
				};
			}
			// Wrap factory
			return container.GenericWrapFactory(factory, reuseType);
		}

		/// <summary>
		/// Build factory from original factory and reuse type<br/>
		/// Return genericFactory for Func&lt;T&gt; and objectFactory for Func&lt;object&gt;<br/>
		/// 根据原工厂函数和重用类型构建新的工厂函数<br/>
		/// 返回Func&lt;T&gt;类型的genericFactory和Func&lt;object&gt;类型的objectFactory<br/>
		/// </summary>
		/// <seealso cref="GenericWrapFactory{T}(IContainer, Func{T}, ReuseType)"/>
		public static void NonGenericWrapFactory(
			this IContainer container, Type type, Func<object> originalFactory, ReuseType reuseType,
			out object genericFactory, out Func<object> objectFactory) {
			if (type.IsGenericTypeDefinition) {
				throw new NotSupportedException("Register generic definition with factory is unsupported");
			}
			var makeObjectFactory = ReflectionUtils.MakeInvoker(GenericWrapFactoryMethod, typeof(object));
			var makeGenericFactory = ReflectionUtils.MakeInvoker(ToGenericFactoryMethod, type);
			objectFactory = (Func<object>)makeObjectFactory(null, new object[] { container, originalFactory, reuseType });
			genericFactory = makeGenericFactory(null, new object[] { objectFactory });
		}

		/// <summary>
		/// Build factory from type and wrap it with reuse type<br/>
		/// Return genericFactory for Func&lt;T&gt; and objectFactory for Func&lt;object&gt;<br/>
		/// Please cache result from this method for better performance<br/>
		/// 根据类型和重用类型构建工厂函数<br/>
		/// 返回Func&lt;T&gt;类型的genericFactory和Func&lt;object&gt;类型的objectFactory<br/>
		/// 请缓存这个函数的结果, 以得到更好的性能<br/>
		/// </summary>
		public static void NonGenericBuildAndWrapFactory(
			this IContainer container, Type type, ReuseType reuseType,
			out object genericFactory, out Func<object> objectFactory) {
			if (type.IsGenericTypeDefinition) {
				// Register Implementation<T> to Service<T>
				var factoryCache = new ConcurrentDictionary<Type, Func<object>>();
				var factoryFactory = new Func<Func<Type, object>>(() => {
					return new Func<Type, object>(serviceType => {
						var bindType = type.MakeGenericType(serviceType.GetGenericArguments());
						var factory = factoryCache.GetOrAdd(serviceType, _ => {
							var makeGenericFactory = ReflectionUtils.MakeInvoker(
								GenericBuildAndWrapFactoryMethod, bindType);
							var makeObjectFactory = ReflectionUtils.MakeInvoker(
								ToObjectFactoryMethod, bindType);
							var innerGenericFactory = makeGenericFactory(
								null, new object[] { container, reuseType });
							var innerObjectFactory = (Func<object>)makeObjectFactory(
								null, new object[] { innerGenericFactory });
							return innerObjectFactory;
						});
						return factory();
					});
				});
				objectFactory = factoryFactory;
				genericFactory = objectFactory;
			} else {
				// Normal case
				var makeGenericFactory = ReflectionUtils.MakeInvoker(GenericBuildAndWrapFactoryMethod, type);
				var makeObjectFactory = ReflectionUtils.MakeInvoker(ToObjectFactoryMethod, type);
				genericFactory = makeGenericFactory(null, new object[] { container, reuseType });
				objectFactory = (Func<object>)makeObjectFactory(null, new object[] { genericFactory });
			}
		}
	}
}
