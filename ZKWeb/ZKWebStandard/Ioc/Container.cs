using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Ioc.Extensions;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// IoC container<br/>
	/// Support constructor dependency injection<br/>
	/// IoC容器<br/>
	/// 支持构造函数注入<br/>
	/// Benchmark (性能测试)<br/>
	/// ZKWeb: 2.0<br/>
	/// DryIoc: 2.11<br/>
	/// - Register Transient: 1000000/2.04s (DryIoc: 5.76s)
	/// - Register Singleton: 1000000/2.18s (DryIoc: 5.76s)
	/// - Resolve Transient: 100000000/2.17s (DryIoc: 1.55s)
	/// - Resolve Singleton: 100000000/2.16s (DryIoc: 1.49s)
	/// - ResolveMany Transient: 10000000/0.86s (DryIoc: 21.01s)
	/// - ResolveMany Singleton: 10000000/0.88s (DryIoc: 7.97s)
	/// </summary>
	/// <seealso cref="IContainer"/>
	/// <seealso cref="IRegistrator"/>
	/// <seealso cref="IGenericRegistrator"/>
	/// <seealso cref="IResolver"/>
	/// <seealso cref="IGenericResolver"/>
	/// <example>
	/// <code language="cs">
	/// void Example() {
	/// 	var animals = Application.Ioc.ResolveMany&lt;IAnimal&gt;()
	/// 	// animals contains instances of Dog and Cow
	///
	/// 	var animalManager = Application.Ioc.Resolve&lt;IAnimalManager&gt;();
	/// 	// animalManager is AnimalManager
	/// 	
	/// 	var otherAnimalManager = Application.Ioc.Resolve&lt;IAnimalManager&gt;();
	/// 	// animalManager only create once, otherAnimalManager == animalManager
	/// }
	///
	/// public interface IAnimal { }
	///
	/// [ExportMany]
	/// public class Dog : IAnimal { }
	///
	/// [ExportMany]
	/// public class Cow : IAnimal { }
	///
	/// public interface IAnimalManager { }
	///
	/// [ExportMany, SingletonUsage]
	/// public class AnimalManager : IAnimalManager {
	/// 	// inject animals
	/// 	public AnimalManager(IEnumerable&lt;IAnimal&gt; animals) { }
	/// }
	///	/// </code>
	///	/// </example>
	public class Container : IContainer {
		/// <summary>
		/// Factories<br/>
		/// 工厂函数的集合<br/>
		/// { (Service Type, Service Key): [Factory Data] }
		/// </summary>
		protected Dictionary<Pair<Type, object>, List<ContainerFactoryData>> Factories { get; set; }
		/// <summary>
		/// Factories lock<br/>
		/// 访问工厂函数的集合时使用的锁<br/> 
		/// </summary>
		protected ReaderWriterLockSlim FactoriesLock { get; set; }
		/// <summary>
		/// Cache for FactoryData<br/>
		/// 工厂函数的缓存<br/>
		/// { (Implementation Type, Reuse Type): Factory Data }
		/// </summary>
		protected ConcurrentDictionary<Pair<Type, ReuseType>, ContainerFactoryData> FactoryDataCache { get; set; }
		/// <summary>
		/// Objects needs to call Dispose when scope finished<br/>
		/// 储存当前范围中需要调用Dispose函数的对象的队列<br/>
		/// </summary>
		protected AsyncLocal<ConcurrentQueue<IDisposable>> ScopedDisposeQueue { get; set; }
		/// <summary>
		/// Increase after each modification<br/>
		/// 更新后自动增加<br/>
		/// </summary>
		internal protected int Revision;

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public Container() {
			Factories = new Dictionary<Pair<Type, object>, List<ContainerFactoryData>>();
			FactoriesLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
			FactoryDataCache = new ConcurrentDictionary<Pair<Type, ReuseType>, ContainerFactoryData>();
			ScopedDisposeQueue = new AsyncLocal<ConcurrentQueue<IDisposable>>();
			Revision = 0;
			RegisterSelf();
		}

		/// <summary>
		/// Get base types and interfaces<br/>
		/// 获取类型的所有基类和接口类<br/>
		/// </summary>
		public static IEnumerable<Type> GetImplementedTypes(Type type) {
			foreach (var interfaceType in type.GetInterfaces()) {
				yield return interfaceType;
			}
			var baseType = type;
			while (baseType != null) {
				yield return baseType;
				baseType = baseType.BaseType;
			}
		}

		/// <summary>
		/// Get base types and interfaces that can be a service type<br/>
		/// What type can't be a service type<br/>
		/// - It's non public and parameter nonPublicServiceTypes is false<br/>
		/// - It's from mscorlib<br/>
		/// 获取类型的可以作为服务类型的基类和接口类<br/>
		/// 什么类型不能作为服务类型<br/>
		/// - 类型是非公开, 并且参数nonPublicServiceTypes是false<br/>
		/// - 类型来源于mscorlib<br/>
		/// </summary>
		public static IEnumerable<Type> GetImplementedServiceTypes(Type type, bool nonPublicServiceTypes) {
			var mscorlibAssembly = typeof(int).Assembly;
			foreach (var serviceType in GetImplementedTypes(type)) {
				if ((!serviceType.IsNotPublic || nonPublicServiceTypes) &&
					(serviceType.Assembly != mscorlibAssembly)) {
					yield return serviceType;
				}
			}
		}

		/// <summary>
		/// Register self instance<br/>
		/// 注册自身到容器<br/>
		/// </summary>
		protected void RegisterSelf() {
			Unregister<IContainer>(null);
			Unregister<IRegistrator>(null);
			Unregister<IGenericRegistrator>(null);
			Unregister<IResolver>(null);
			Unregister<IGenericResolver>(null);
			RegisterInstance<IContainer>(this, null);
			RegisterInstance<IRegistrator>(this, null);
			RegisterInstance<IGenericRegistrator>(this, null);
			RegisterInstance<IResolver>(this, null);
			RegisterInstance<IGenericResolver>(this, null);
		}

		/// <summary>
		/// Register factory with service type and service key<br/>
		/// 根据服务类型和服务键注册工厂函数<br/>
		/// </summary>
		protected void RegisterFactory(
			Type serviceType, ContainerFactoryData factoryData, object serviceKey) {
			FactoriesLock.EnterWriteLock();
			try {
				var key = Pair.Create(serviceType, serviceKey);
				var factories = Factories.GetOrCreate(key, () => new List<ContainerFactoryData>(1));
				factories.Add(factoryData);
			} finally {
				Interlocked.Increment(ref Revision);
				FactoriesLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Register factory with service types and service key<br/>
		/// 根据多个服务类型和服务键注册工厂函数<br/>
		/// </summary>
		protected void RegisterFactoryMany(
			IEnumerable<Type> serviceTypes, ContainerFactoryData factoryData, object serviceKey) {
			FactoriesLock.EnterWriteLock();
			try {
				foreach (var serviceType in serviceTypes) {
					var key = Pair.Create(serviceType, serviceKey);
					var factories = Factories.GetOrCreate(key, () => new List<ContainerFactoryData>(1));
					factories.Add(factoryData);
				}
			} finally {
				Interlocked.Increment(ref Revision);
				FactoriesLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Register implementation type with service type and service key<br/>
		/// 根据服务类型和服务键注册实现类型<br/>
		/// </summary>
		public void Register(
			Type serviceType, Type implementationType, ReuseType reuseType, object serviceKey) {
			var factoryData = FactoryDataCache.GetOrAdd(
				Pair.Create(implementationType, reuseType),
				pair => {
					this.NonGenericBuildAndWrapFactory(
						pair.First, pair.Second, out var genericFactory, out var objectFactory);
					return new ContainerFactoryData(genericFactory, objectFactory, pair.First);
				});
			RegisterFactory(serviceType, factoryData, serviceKey);
		}

		/// <summary>
		/// Register implementation type with service type and service key<br/>
		/// 根据服务类型和服务键注册实现类型<br/>
		/// </summary>
		public void Register<TService, TImplementation>(ReuseType reuseType, object serviceKey) {
			var factoryData = FactoryDataCache.GetOrAdd(
				Pair.Create(typeof(TImplementation), reuseType),
				pair => {
					var genericFactory = this.GenericBuildAndWrapFactory<TImplementation>(pair.Second);
					var objectFactory = new Func<object>(() => genericFactory());
					return new ContainerFactoryData(genericFactory, objectFactory, typeof(TImplementation));
				});
			RegisterFactory(typeof(TService), factoryData, serviceKey);
		}

		/// <summary>
		/// Register implementation type with service types and service key<br/>
		/// 根据多个服务类型和服务键注册实现类型<br/>
		/// </summary>
		public void RegisterMany(
			IList<Type> serviceTypes, Type implementationType, ReuseType reuseType, object serviceKey) {
			var factoryData = FactoryDataCache.GetOrAdd(
				Pair.Create(implementationType, reuseType),
				pair => {
					this.NonGenericBuildAndWrapFactory(
						pair.First, pair.Second, out var genericFactory, out var objectFactory);
					return new ContainerFactoryData(genericFactory, objectFactory, pair.First);
				});
			RegisterFactoryMany(serviceTypes, factoryData, serviceKey);
		}

		/// <summary>
		/// Register implementation type with service types and service key<br/>
		/// Service types are obtain from base types and interfaces<br/>
		/// 根据多个服务类型和服务键注册实现类型<br/>
		/// 服务类型是实现类型的基类和接口<br/>
		/// </summary>
		public void RegisterMany(Type implementationType,
			ReuseType reuseType, object serviceKey, bool nonPublicServiceTypes) {
			var serviceTypes = GetImplementedServiceTypes(
				implementationType, nonPublicServiceTypes).ToList();
			RegisterMany(serviceTypes, implementationType, reuseType, serviceKey);
		}

		/// <summary>
		/// Register implementation type with service types and service key<br/>
		/// Service types are obtain from base types and interfaces<br/>
		/// 根据多个服务类型和服务键注册实现类型<br/>
		/// 服务类型是实现类型的基类和接口<br/>
		/// </summary>
		public void RegisterMany<TImplementation>(
			ReuseType reuseType, object serviceKey, bool nonPublicServiceTypes) {
			var factoryData = FactoryDataCache.GetOrAdd(
				Pair.Create(typeof(TImplementation), reuseType),
				pair => {
					var genericFactory = this.GenericBuildAndWrapFactory<TImplementation>(pair.Second);
					var objectFactory = new Func<object>(() => genericFactory());
					return new ContainerFactoryData(genericFactory, objectFactory, typeof(TImplementation));
				});
			var serviceTypes = GetImplementedServiceTypes(typeof(TImplementation), nonPublicServiceTypes);
			RegisterFactoryMany(serviceTypes, factoryData, serviceKey);
		}

		/// <summary>
		/// Register instance with service type and service key<br/>
		/// Reuse type is forced to Singleton<br/>
		/// 根据服务类型和服务键注册实例<br/>
		/// 重用类型强制为单例<br/>
		/// </summary>
		public void RegisterInstance(Type serviceType, object instance, object serviceKey) {
			var implementationType = instance.GetType();
			var factoryData = FactoryDataCache.GetOrAdd(
				Pair.Create(implementationType, ReuseType.Singleton),
				pair => {
					this.NonGenericWrapFactory(implementationType,
						() => instance, pair.Second, out var genericFactory, out var objectFactory);
					return new ContainerFactoryData(genericFactory, objectFactory, pair.First);
				});
			RegisterFactory(serviceType, factoryData, serviceKey);
		}

		/// <summary>
		/// Register instance with service type and service key<br/>
		/// Reuse type is forced to Singleton<br/>
		/// 根据服务类型和服务键注册实例<br/>
		/// 重用类型强制为单例<br/>
		/// </summary>
		public void RegisterInstance<TService>(TService instance, object serviceKey) {
			var implementationType = instance.GetType();
			var factoryData = FactoryDataCache.GetOrAdd(
				Pair.Create(implementationType, ReuseType.Singleton),
				pair => {
					var genericFactory = this.GenericWrapFactory<TService>(() => instance, pair.Second);
					var objectFactory = new Func<object>(() => genericFactory());
					return new ContainerFactoryData(genericFactory, objectFactory, pair.First);
				});
			RegisterFactory(typeof(TService), factoryData, serviceKey);
		}

		/// <summary>
		/// Register delegate with service type and service key<br/>
		/// 根据服务类型和服务键注册工厂函数<br/>
		/// </summary>
		public void RegisterDelegate(
			Type serviceType, Func<object> factory, ReuseType reuseType, object serviceKey) {
			// We can't figure out what type will returned from factory, the hint type will be the service type
			var factoryData = FactoryDataCache.GetOrAdd(
				Pair.Create(serviceType, reuseType),
				pair => {
					this.NonGenericWrapFactory(
						pair.First, factory, pair.Second, out var genericFactory, out var objectFactory);
					return new ContainerFactoryData(genericFactory, objectFactory, pair.First);
				});
			RegisterFactory(serviceType, factoryData, serviceKey);
		}

		/// <summary>
		/// Register delegate with service type and service key<br/>
		/// 根据服务类型和服务键注册工厂函数<br/>
		/// </summary>
		public void RegisterDelegate<TService>(
			Func<TService> factory, ReuseType reuseType, object serviceKey) {
			// We can't figure out what type will returned from factory, the hint type will be the service type
			var factoryData = FactoryDataCache.GetOrAdd(
				Pair.Create(typeof(TService), reuseType),
				pair => {
					var genericFactory = this.GenericWrapFactory<TService>(factory, pair.Second);
					var objectFactory = new Func<object>(() => genericFactory());
					return new ContainerFactoryData(genericFactory, objectFactory, pair.First);
				});
			RegisterFactory(typeof(TService), factoryData, serviceKey);
		}

		/// <summary>
		/// Automatic register types by export attributes<br/>
		/// 按Export属性自动注册类型到容器<br/>
		/// </summary>
		public void RegisterExports(IEnumerable<Type> types) {
			foreach (var type in types) {
				// Get export attributes
				var exportAttributes = type.GetAttributes<ExportAttributeBase>();
				if (!exportAttributes.Any()) {
					continue;
				}
				// Get reuse attribute
				var reuseType = type.GetAttribute<ReuseAttribute>()?.ReuseType ?? default(ReuseType);
				// Call RegisterToContainer
				foreach (var attribute in exportAttributes) {
					attribute.RegisterToContainer(this, type, reuseType);
				}
			}
		}

		/// <summary>
		/// Unregister all factories with specified service type and service key<br/>
		/// 注销所有注册到指定服务类型和服务键的工厂函数<br/>
		/// </summary>
		public void Unregister(Type serviceType, object serviceKey) {
			var key = Pair.Create(serviceType, serviceKey);
			FactoriesLock.EnterWriteLock();
			try {
				Factories.Remove(key);
			} finally {
				Interlocked.Increment(ref Revision);
				FactoriesLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Unregister all factories with specified service type and service key<br/>
		/// 注销所有注册到指定服务类型和服务键的工厂函数<br/>
		/// </summary>
		public void Unregister<TService>(object serviceKey) {
			Unregister(typeof(TService), serviceKey);
		}

		/// <summary>
		/// Unregister factories with specified implementation type and service key<br/>
		/// 按实现类型和服务键注销所有相关的工厂函数<br/>
		/// </summary>
		public void UnregisterImplementation(Type implementationType, object serviceKey) {
			var serviceTypes = GetImplementedServiceTypes(implementationType, true).ToList();
			var keys = serviceTypes.Select(t => Pair.Create(t, serviceKey)).ToList();
			FactoriesLock.EnterWriteLock();
			try {
				foreach (var key in keys) {
					var factories = Factories.GetOrDefault(key);
					factories?.RemoveAll(f => f.ImplementationTypeHint == implementationType);
				}
			} finally {
				Interlocked.Increment(ref Revision);
				FactoriesLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Unregister factories with specified implementation type and service key<br/>
		/// 按实现类型和服务键注册所有相关的工厂函数<br/>
		/// </summary>
		public void UnregisterImplementation<TImplementation>(object serviceKey) {
			UnregisterImplementation(typeof(TImplementation), serviceKey);
		}

		/// <summary>
		/// Unregister all factories<br/>
		/// 注销所有工厂函数<br/>
		/// </summary>
		public void UnregisterAll() {
			FactoriesLock.EnterWriteLock();
			try {
				Factories.Clear();
			} finally {
				Interlocked.Increment(ref Revision);
				FactoriesLock.ExitWriteLock();
			}
			GC.Collect();
		}

		/// <summary>
		/// Update factories cache for service type and return newest data<br/>
		/// 根据服务类型更新工厂函数的缓存并返回最新的数据<br/>
		/// </summary>
		internal ContainerFactoriesCacheData<TService> UpdateFactoriesCache<TService>() {
			var key = Pair.Create(typeof(TService), (object)null);
			var factoriesCopy = new List<Func<TService>>();
			FactoriesLock.EnterReadLock();
			try {
				var factories = Factories.GetOrDefault(key);
				if (factories != null) {
					factoriesCopy.Capacity = factories.Count;
					factoriesCopy.AddRange(factories.Select(f => (Func<TService>)f.GenericFactory));
				}
			} finally {
				FactoriesLock.ExitReadLock();
			}
			var data = new ContainerFactoriesCacheData<TService>(this, factoriesCopy);
			ContainerFactoriesCache<TService>.Data = data;
			return data;
		}

		/// <summary>
		/// Resolve service with type and key<br/>
		/// Throw exception or return default value if not found, dependent on ifUnresolved<br/>
		/// 根据服务类型和服务键获取实例<br/>
		/// 找不到时根据ifUnresolved参数抛出例外或者返回默认值<br/>
		/// </summary>
		public object Resolve(Type serviceType, IfUnresolved ifUnresolved, object serviceKey) {
			var key = Pair.Create(serviceType, serviceKey);
			Func<object> factory;
			long factoriesCount;
			// Get factories
			// Success unless there zero or more than one factories
			FactoriesLock.EnterReadLock();
			try {
				var factories = Factories.GetOrDefault(key);
				if (factories == null && serviceType.IsGenericType) {
					// Use factory from generic definition
					var baseKey = Pair.Create(serviceType.GetGenericTypeDefinition(), serviceKey);
					factories = Factories.GetOrDefault(baseKey);
					factoriesCount = factories?.Count ?? 0;
					if (factoriesCount == 1) {
						var factoryData = factories[0];
						factory = () => ((Func<Type, object>)factoryData.ObjectFactory.Invoke())(serviceType);
					} else {
						factory = null;
					}
				} else {
					// Use normal factory
					factoriesCount = factories?.Count ?? 0;
					if (factoriesCount == 1) {
						var factoryData = factories[0];
						factory = factoryData.ObjectFactory;
					} else {
						factory = null;
					}
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
				return null;
			} else {
				throw new NotSupportedException(string.Format(
					"unsupported ifUnresolved type {0}", ifUnresolved));
			}
		}

		/// <summary>
		/// Resolve service with type and key<br/>
		/// Throw exception or return default value if not found, dependent on ifUnresolved<br/>
		/// 根据服务类型和服务键获取实例<br/>
		/// 找不到时根据ifUnresolved参数抛出例外或者返回默认值<br/>
		/// </summary>
		public TService Resolve<TService>(IfUnresolved ifUnresolved, object serviceKey) {
			if (serviceKey == null && ContainerFactoriesCache.Enabled) {
				// Use faster method
				var data = ContainerFactoriesCache<TService>.Data;
				if (data == null || !data.IsMatched(this)) {
					data = UpdateFactoriesCache<TService>();
				}
				if (data.SingleFactory != null) {
					return data.SingleFactory();
				}
			}
			// Use default method
			return (TService)(Resolve(typeof(TService), ifUnresolved, serviceKey) ?? default(TService));
		}

		/// <summary>
		/// Resolve services with type and key<br/>
		/// Return empty sequence if no service registered<br/>
		/// 根据服务类型和服务键获取实例列表<br/>
		/// 如果无注册的服务则返回空列表<br/>
		/// </summary>
		public IEnumerable<object> ResolveMany(Type serviceType, object serviceKey) {
			var key = Pair.Create(serviceType, serviceKey);
			var factoriesCopy = new List<Func<object>>();
			// Copy factories
			FactoriesLock.EnterReadLock();
			try {
				var factories = Factories.GetOrDefault(key);
				if (factories == null && serviceType.IsGenericType) {
					// Use factories from generic definition
					var baseKey = Pair.Create(serviceType.GetGenericTypeDefinition(), serviceKey);
					factories = Factories.GetOrDefault(baseKey);
					if (factories != null) {
						factoriesCopy.Capacity = factories.Count;
						factoriesCopy.AddRange(factories.Select(f => new Func<object>(() =>
							((Func<Type, object>)f.ObjectFactory.Invoke())(serviceType))));
					}
				} else if (factories != null) {
					// Use normal factories
					factoriesCopy.Capacity = factories.Count;
					factoriesCopy.AddRange(factories.Select(f => f.ObjectFactory));
				}
			} finally {
				FactoriesLock.ExitReadLock();
			}
			// Get service instances
			foreach (var factory in factoriesCopy) {
				yield return factory();
			}
		}

		/// <summary>
		/// Resolve services with type and key<br/>
		/// Return empty sequence if no service registered<br/>
		/// 根据服务类型和服务键获取服务列表<br/>
		/// 如果无注册的服务则返回空列表<br/>
		/// </summary>
		public IEnumerable<TService> ResolveMany<TService>(object serviceKey) {
			if (serviceKey == null && ContainerFactoriesCache.Enabled) {
				// Use faster method
				var data = ContainerFactoriesCache<TService>.Data;
				if (data == null || !data.IsMatched(this)) {
					data = UpdateFactoriesCache<TService>();
				}
				if (data.Factories.Count > 0) {
					foreach (var factory in data.Factories) {
						yield return factory();
					}
					yield break;
				}
			}
			// Use default method
			foreach (var instance in ResolveMany(typeof(TService), serviceKey)) {
				yield return (TService)instance;
			}
		}

		/// <summary>
		/// 根据服务类型和服务键获取工厂函数<br/>
		/// 如果无注册的服务则返回空列表<br/>
		/// </summary>
		public IEnumerable<ContainerFactoryData> ResolveFactories(
			Type serviceType, object serviceKey) {
			var key = Pair.Create(serviceType, serviceKey);
			var factoriesCopy = new List<ContainerFactoryData>();
			// Copy factories
			FactoriesLock.EnterReadLock();
			try {
				var factories = Factories.GetOrDefault(key);
				if (factories != null) {
					factoriesCopy.Capacity = factories.Count;
					factoriesCopy.AddRange(factories);
				}
			} finally {
				FactoriesLock.ExitReadLock();
			}
			// Return Copied
			return factoriesCopy;
		}

		/// <summary>
		/// 根据服务类型和服务键获取工厂函数<br/>
		/// 如果无注册的服务则返回空列表<br/>
		/// </summary>
		public IEnumerable<ContainerFactoryData> ResolveFactories<TService>(object serviceKey) {
			return ResolveFactories(typeof(TService), serviceKey);
		}

		/// <summary>
		/// Dispose specific object when scope finished<br/>
		/// 在范围结束后调用指定对象的Dispose函数<br/>
		/// </summary>
		public void DisposeWhenScopeFinished(IDisposable disposable) {
			var queue = ScopedDisposeQueue.Value;
			if (queue == null) {
				lock (ScopedDisposeQueue) {
					queue = ScopedDisposeQueue.Value;
					if (queue == null) {
						ScopedDisposeQueue.Value = queue = new ConcurrentQueue<IDisposable>();
					}
				}
			}
			queue.Enqueue(disposable);
		}

		/// <summary>
		/// Notify scope finished<br/>
		/// 通知当前范围已结束<br/>
		/// </summary>
		public void ScopeFinished() {
			ConcurrentQueue<IDisposable> queue;
			lock (ScopedDisposeQueue) {
				queue = ScopedDisposeQueue.Value;
				ScopedDisposeQueue.Value = null;
			}
			if (queue != null) {
				foreach (var obj in queue) {
					obj.Dispose();
				}
			}
		}

		/// <summary>
		/// Clone container<br/>
		/// 克隆容器<br/>
		/// </summary>
		/// <returns></returns>
		public object Clone() {
			var clone = new Container();
			foreach (var pair in Factories) {
				// BUG: Constructor injection will still reference to old container
				// Make factory take parameter should resolve this problem but will impact performance
				clone.Factories[pair.Key] = new List<ContainerFactoryData>(pair.Value);
			}
			clone.RegisterSelf(); // replace self instances
			clone.Revision = Revision; // inherit revision
			return clone;
		}

		/// <summary>
		/// Dispose container<br/>
		/// 清理容器的资源<br/>
		/// </summary>
		public void Dispose() {
			GC.Collect();
		}
	}
}
