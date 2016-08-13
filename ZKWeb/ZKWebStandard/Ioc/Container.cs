using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// IoC container
	/// Support constructor dependency injection
	/// Benchmark
	/// - Register Transient: 1000000/2.3s (DryIoc: 6.1s)
	/// - Register Singleton: 1000000/2.6s (DryIoc: 5.2s)
	/// - Resolve Transient: 10000000/3.2s (DryIoc: 0.54s)
	/// - Resolve Singleton: 10000000/3.2s (DryIoc: 0.43s)
	/// - ResolveMany Transient: 10000000/11.8s (DryIoc: 14.7s)
	/// - ResolveMany Singleton: 10000000/11.1s (DryIoc: 12.9s)
	/// </summary>
	public class Container : IContainer {
		/// <summary>
		/// Factories
		/// { (Type, Service key): [Factory] }
		/// </summary>
		protected IDictionary<Pair<Type, object>, IList<Func<object>>> Factories { get; set; }
		/// <summary>
		/// Factories lock
		/// </summary>
		protected ReaderWriterLockSlim FactoriesLock { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public Container() {
			Factories = new Dictionary<Pair<Type, object>, IList<Func<object>>>();
			FactoriesLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
		}

		/// <summary>
		/// Build factory from original factory and reuse type
		/// </summary>
		protected static Func<object> BuildFactory(Func<object> originalFactory, ReuseType reuseType) {
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
			} else {
				throw new NotSupportedException(string.Format("unsupported reuse type {0}", reuseType));
			}
		}

		/// <summary>
		/// Cache for type and it's original factory
		/// </summary>
		protected readonly static ConcurrentDictionary<Type, Func<object>> TypeFactorysCache =
			new ConcurrentDictionary<Type, Func<object>>();

		/// <summary>
		/// Build factory from type and reuse type
		/// </summary>
		protected static Func<object> BuildFactor(
			IContainer container, Type type, ReuseType reuseType) {
			var typeFactor = TypeFactorysCache.GetOrAdd(type, t => {
				// Support constructor dependency injection
				var argumentExpressions = new List<Expression>();
				var constructor = t.GetConstructors().Where(c => c.IsPublic).First();
				foreach (var parameter in constructor.GetParameters()) {
					var parameterType = parameter.ParameterType;
					var parameterTypeInfo = parameterType.GetTypeInfo();
					if (parameterTypeInfo.IsGenericType &&
						parameterTypeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
						argumentExpressions.Add(Expression.Call(
							Expression.Constant(container), "ResolveMany",
							parameterTypeInfo.GetGenericArguments(),
							Expression.Constant(null)));
					} else {
						argumentExpressions.Add(Expression.Call(
							Expression.Constant(container), "Resolve",
							new[] { parameterType },
							Expression.Constant(IfUnresolved.Throw),
							Expression.Constant(null)));
					}
				}
				var newExpression = Expression.New(constructor, argumentExpressions);
				return Expression.Lambda<Func<object>>(newExpression).Compile();
			});
			return BuildFactory(typeFactor, reuseType);
		}

		/// <summary>
		/// Get base types and interfaces
		/// </summary>
		protected static IEnumerable<Type> GetImplementedTypes(Type type) {
			foreach (var interfaceType in type.GetTypeInfo().GetInterfaces()) {
				yield return interfaceType;
			}
			var baseType = type;
			while (baseType != null) {
				yield return baseType;
				baseType = baseType.GetTypeInfo().BaseType;
			}
		}

		/// <summary>
		/// Get base types and interfaces that can be a service type
		/// What type can't be a service type
		/// - It's non public and parameter nonPublicServiceTypes is false
		/// - It's from mscorlib
		/// </summary>
		protected static IEnumerable<Type> GetImplementedServiceTypes(Type type, bool nonPublicServiceTypes) {
			var mscorlibAssembly = typeof(int).GetTypeInfo().Assembly;
			foreach (var serviceType in GetImplementedTypes(type)) {
				var serviceTypeInfo = serviceType.GetTypeInfo();
				if ((!serviceTypeInfo.IsNotPublic || nonPublicServiceTypes) &&
					(serviceTypeInfo.Assembly != mscorlibAssembly)) {
					yield return serviceType;
				}
			}
		}

		/// <summary>
		/// Register factory with service type and service key
		/// </summary>
		protected void RegisterFactory(Type serviceType, Func<object> factory, object serviceKey) {
			var key = Pair.Create(serviceType, serviceKey);
			FactoriesLock.EnterWriteLock();
			try {
				var factors = Factories.GetOrCreate(key, () => new List<Func<object>>());
				factors.Add(factory);
			} finally {
				FactoriesLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Register factory with service types and service key
		/// </summary>
		protected void RegisterFactoryMany(IList<Type> serviceTypes, Func<object> factory, object serviceKey) {
			FactoriesLock.EnterWriteLock();
			try {
				foreach (var serviceType in serviceTypes) {
					var key = Pair.Create(serviceType, serviceKey);
					var factories = Factories.GetOrCreate(key, () => new List<Func<object>>());
					factories.Add(factory);
				}
			} finally {
				FactoriesLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Register implementation type with service type and service key
		/// </summary>
		public void Register(
			Type serviceType, Type implementationType, ReuseType reuseType, object serviceKey) {
			var factory = BuildFactor(this, implementationType, reuseType);
			RegisterFactory(serviceType, factory, serviceKey);
		}

		/// <summary>
		/// Register implementation type with service type and service key
		/// </summary>
		public void Register<TService, TImplementation>(ReuseType reuseType, object serviceKey) {
			Register(typeof(TService), typeof(TImplementation), reuseType, serviceKey);
		}

		/// <summary>
		/// Register implementation type with service types and service key
		/// </summary>
		public void RegisterMany(
			IList<Type> serviceTypes, Type implementationType, ReuseType reuseType, object serviceKey) {
			var factory = BuildFactor(this, implementationType, reuseType);
			RegisterFactoryMany(serviceTypes, factory, serviceKey);
		}

		/// <summary>
		/// Register implementation type with service types and service key
		/// Service types are obtain from base types and interfaces
		/// </summary>
		public void RegisterMany(Type implementationType,
			ReuseType reuseType, object serviceKey, bool nonPublicServiceTypes) {
			var serviceTypes = GetImplementedServiceTypes(
				implementationType, nonPublicServiceTypes).ToList();
			RegisterMany(serviceTypes, implementationType, reuseType, serviceKey);
		}

		/// <summary>
		/// Register implementation type with service types and service key
		/// Service types are obtain from base types and interfaces
		/// </summary>
		public void RegisterMany<TImplementation>(
			ReuseType reuseType, object serviceKey, bool nonPublicServiceTypes) {
			RegisterMany(typeof(TImplementation), reuseType, serviceKey, nonPublicServiceTypes);
		}

		/// <summary>
		/// Register instance with service type and service key
		/// Reuse type is forced to Singleton
		/// </summary>
		public void RegisterInstance(Type serviceType, object instance, object serviceKey) {
			var factory = BuildFactory(() => instance, ReuseType.Singleton);
			RegisterFactory(serviceType, factory, serviceKey);
		}

		/// <summary>
		/// Register instance with service type and service key
		/// Reuse type is forced to Singleton
		/// </summary>
		public void RegisterInstance<TService>(TService instance, object serviceKey) {
			RegisterInstance(typeof(TService), instance, serviceKey);
		}

		/// <summary>
		/// Register delegate with service type and service key
		/// </summary>
		public void RegisterDelegate(
			Type serviceType, Func<object> factory, ReuseType reuseType, object serviceKey) {
			factory = BuildFactory(factory, reuseType);
			RegisterFactory(serviceType, factory, serviceKey);
		}

		/// <summary>
		/// Register delegate with service type and service key
		/// </summary>
		public void RegisterDelegate<TService>(
			Func<TService> factory, ReuseType reuseType, object serviceKey) {
			RegisterDelegate(typeof(TService), () => factory(), reuseType, serviceKey);
		}

		/// <summary>
		/// Automatic register types by export attributes
		/// </summary>
		public void RegisterExports(IEnumerable<Type> types) {
			foreach (var type in types) {
				var typeInfo = type.GetTypeInfo();
				var exportManyAttribute = typeInfo.GetAttribute<ExportManyAttribute>();
				if (exportManyAttribute == null) {
					continue;
				}
				// From ExportManyAttribute
				var reuseType = typeInfo.GetAttribute<ReuseAttribute>()?.ReuseType ?? default(ReuseType);
				var nonPublic = exportManyAttribute.NonPublic;
				var except = exportManyAttribute.Except;
				var contractKey = exportManyAttribute.ContractKey;
				var serviceTypes = GetImplementedServiceTypes(type, nonPublic);
				if (except != null && except.Any()) {
					serviceTypes = serviceTypes.Where(t => !except.Contains(t));
				}
				RegisterMany(serviceTypes.ToList(), type, reuseType, contractKey);
			}
		}

		/// <summary>
		/// Unregister all factories with specified service type and service key
		/// </summary>
		public void Unregister(Type serviceType, object serviceKey) {
			var key = Pair.Create(serviceType, serviceKey);
			FactoriesLock.EnterWriteLock();
			try {
				Factories.Remove(key);
			} finally {
				FactoriesLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Unregister all factories with specified service type and service key
		/// </summary>
		public void Unregister<TService>(object serviceKey) {
			Unregister(typeof(TService), serviceKey);
		}

		/// <summary>
		/// Unregister all factories
		/// </summary>
		public void UnregisterAll() {
			FactoriesLock.EnterWriteLock();
			try {
				Factories.Clear();
			} finally {
				FactoriesLock.ExitWriteLock();
			}
			GC.Collect();
		}

		/// <summary>
		/// Resolve service with type and key
		/// Throw exception or return default value if not found, dependent on ifUnresolved
		/// </summary>
		public object Resolve(Type serviceType, IfUnresolved ifUnresolved, object serviceKey) {
			var key = Pair.Create(serviceType, serviceKey);
			Func<object> factory = null;
			long factoriesCount = 0;
			FactoriesLock.EnterReadLock();
			try {
				// Get factories
				// Only success if there single factory
				var factories = Factories.GetOrDefault(key);
				factoriesCount = factories?.Count ?? 0;
				if (factoriesCount == 1) {
					factory = factories[0];
				}
			} finally {
				FactoriesLock.ExitReadLock();
			}
			if (factory != null) {
				// Success
				return factory();
			} else if (ifUnresolved == IfUnresolved.Throw) {
				// Error, throw exception
				var messageFormat = (factoriesCount <= 0) ?
					"no factory registered to type {0} and service key {1}" :
					"more than one factory registered to type {0} and service key {1}";
				throw new KeyNotFoundException(string.Format(messageFormat, serviceType, serviceKey));
			} else if (ifUnresolved == IfUnresolved.ReturnDefault) {
				// Error, return default value
				return serviceType.GetTypeInfo().IsValueType ?
					Activator.CreateInstance(serviceType) : null;
			} else {
				throw new NotSupportedException(string.Format(
					"unsupported ifUnresolved type {0}", ifUnresolved));
			}
		}

		/// <summary>
		/// Resolve service with type and key
		/// Throw exception or return default value if not found, dependent on ifUnresolved
		/// </summary>
		public TService Resolve<TService>(IfUnresolved ifUnresolved, object serviceKey) {
			return (TService)Resolve(typeof(TService), ifUnresolved, serviceKey);
		}

		/// <summary>
		/// Resolve services with type and key
		/// Return empty sequence if no service registered
		/// </summary>
		public IEnumerable<object> ResolveMany(Type serviceType, object serviceKey) {
			var key = Pair.Create(serviceType, serviceKey);
			var factorsCopy = new List<Func<object>>();
			FactoriesLock.EnterReadLock();
			try {
				// Copy factories
				var factors = Factories.GetOrDefault(key);
				if (factors != null) {
					factorsCopy.Capacity = factors.Count;
					factorsCopy.AddRange(factors);
				}
			} finally {
				FactoriesLock.ExitReadLock();
			}
			// Get service instances
			foreach (var factory in factorsCopy) {
				yield return factory();
			}
		}

		/// <summary>
		/// Resolve services with type and key
		/// Return empty sequence if no service registered
		/// </summary>
		public IEnumerable<TService> ResolveMany<TService>(object serviceKey) {
			foreach (var instance in ResolveMany(typeof(TService), serviceKey)) {
				yield return (TService)instance;
			}
		}

		/// <summary>
		/// Clone container
		/// </summary>
		/// <returns></returns>
		public object Clone() {
			var clone = new Container();
			FactoriesLock.EnterReadLock();
			try {
				foreach (var pair in Factories) {
					clone.Factories[pair.Key] = pair.Value.ToList();
				}
			} finally {
				FactoriesLock.ExitReadLock();
			}
			return clone;
		}

		/// <summary>
		/// Dispose container
		/// </summary>
		public void Dispose() {
			GC.Collect();
		}
	}
}
