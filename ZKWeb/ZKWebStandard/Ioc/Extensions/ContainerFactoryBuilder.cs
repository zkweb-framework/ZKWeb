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
		/// <summary>
		/// Cache of factory functions<br/>
		/// 工厂函数的缓存<br/>
		/// </summary>
		private static ConcurrentDictionary<ConstructorInfo, ContainerFactoryDelegate> FactoryCache =
			new ConcurrentDictionary<ConstructorInfo, ContainerFactoryDelegate>();

		/// <summary>
		/// Build factory function from constructor<br/>
		/// 根据构造函数构建工厂函数<br/>
		/// </summary>
		public static ContainerFactoryDelegate BuildFactory(ConstructorInfo constructor) {
			if (FactoryCache.TryGetValue(constructor, out var factoryFunc)) {
				return factoryFunc;
			}
			var containerParameter = Expression.Parameter(typeof(IContainer), "c");
			var serviceTypeParameter = Expression.Parameter(typeof(Type), "s");
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
					// do nothing
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
						Expression.Convert(
							containerParameter,
							typeof(IGenericResolver)),
						nameof(IGenericResolver.ResolveMany),
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
						Expression.Convert(
							containerParameter,
							typeof(IGenericResolver)),
						nameof(IGenericResolver.Resolve),
						new[] { parameterType },
						Expression.Constant(IfUnresolved.Throw),
						Expression.Constant(null));
				} else {
					// Use generic Resolve with ?? operator
					argumentExpression = Expression.Convert(
						Expression.Coalesce(
							Expression.Call(
								Expression.Convert(
									containerParameter,
									typeof(IResolver)),
								nameof(IResolver.Resolve),
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
					var argumentFactory = Expression.Lambda(argumentFactoryType, argumentExpression);
					var lazyConstructor = typeof(Lazy<>).MakeGenericType(argumentExpression.Type).GetConstructors()
						.First(c => c.GetParameters().Length == 1 &&
							c.GetParameters()[0].ParameterType == argumentFactoryType);
					argumentExpression = Expression.New(lazyConstructor, argumentFactory);
				}
				if (isFunc) {
					// Pass factory
					var argumentFactoryType = typeof(Func<>).MakeGenericType(argumentExpression.Type);
					var argumentFactory = Expression.Lambda(argumentFactoryType, argumentExpression);
					argumentExpression = argumentFactory;
				}
				argumentExpressions.Add(argumentExpression);
			}
			var newExpression = Expression.New(constructor, argumentExpressions);
			factoryFunc = Expression.Lambda<ContainerFactoryDelegate>(
				newExpression, containerParameter, serviceTypeParameter).Compile();
			FactoryCache[constructor] = factoryFunc;
			return factoryFunc;
		}

		/// <summary>
		/// Create factory function from type<br/>
		/// 根据类型构建工厂函数<br/>
		/// </summary>
		public static ContainerFactoryDelegate BuildFactory(Type type) {
			if (type.IsGenericTypeDefinition) {
				// Generic type
				return (container, serviceType) => {
					var implementationType = type.MakeGenericType(
						serviceType.GetGenericArguments());
					var factoryFunc = BuildFactory(implementationType);
					return factoryFunc(container, serviceType);
				};
			} else {
				// Non generic type
				// First: use constructor that marked with InjectAttribute
				var constructors = type.GetConstructors();
				var constructor = constructors
					.Where(c => c.GetAttribute<InjectAttribute>() != null).SingleOrDefaultNotThrow();
				if (constructor != null) {
					return BuildFactory(constructor);
				}
				// Second: use the only public constructor
				constructor = constructors.Where(c => c.IsPublic).SingleOrDefaultNotThrow();
				if (constructor != null) {
					return BuildFactory(constructor);
				}
				// Third: use runtime resolver
				return (container, serviceType) => {
					var resolver = container.Resolve<IMultiConstructorResolver>(IfUnresolved.ReturnDefault);
					if (resolver == null) {
						throw new ArgumentException(
							$"Type {type} have multi constructor and no one is marked with [Inject]," +
							"please mark one with [Inject] or provide IMultiConstructorResolver");
					} else {
						return resolver.Resolve(type);
					}
				};
			}
		}
	}
}
