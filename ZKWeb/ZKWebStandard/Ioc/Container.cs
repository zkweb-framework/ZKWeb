using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// IoC container<br/>
	/// Support constructor dependency injection<br/>
	/// IoC容器<br/>
	/// 支持构造函数注入<br/>
	/// Benchmark (性能测试)<br/>
	/// - Register Transient: 1000000/2.3s (DryIoc: 6.1s)
	/// - Register Singleton: 1000000/2.6s (DryIoc: 5.2s)
	/// - Resolve Transient: 10000000/0.27s (DryIoc: 0.54s)
	/// - Resolve Singleton: 10000000/0.27s (DryIoc: 0.43s)
	/// - ResolveMany Transient: 10000000/0.84s (DryIoc: 14.7s)
	/// - ResolveMany Singleton: 10000000/0.88s (DryIoc: 12.9s)
	/// </summary>
	/// <seealso cref="IContainer"/>
	/// <seealso cref="IRegistrator"/>
	/// <seealso cref="IGenericRegistrator"/>
	/// <seealso cref="IResolver"/>
	/// <seealso cref="IGenericResolver"/>
	/// <seealso cref="IContainerExtensions"/>
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
		/// { (Type, Service key): (Factory, Implementation Type) }
		/// </summary>
		protected IDictionary<Pair<Type, object>, List<ContainerFactoryData>> Factories { get; set; }
		/// <summary>
		/// Factories lock<br/>
		/// 访问工厂函数的集合时使用的锁<br/>
		/// </summary>
		protected ReaderWriterLockSlim FactoriesLock { get; set; }
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
			var key = Pair.Create(serviceType, serviceKey);
			FactoriesLock.EnterWriteLock();
			try {
				var factories = Factories.GetOrCreate(key, () => new List<ContainerFactoryData>());
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
			IList<Type> serviceTypes, ContainerFactoryData factoryData, object serviceKey) {
			FactoriesLock.EnterWriteLock();
			try {
				foreach (var serviceType in serviceTypes) {
					var key = Pair.Create(serviceType, serviceKey);
					var factories = Factories.GetOrCreate(key, () => new List<ContainerFactoryData>());
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
			var factory = this.BuildFactory(implementationType, reuseType);
			var factoryData = new ContainerFactoryData(factory, implementationType);
			RegisterFactory(serviceType, factoryData, serviceKey);
		}

		/// <summary>
		/// Register implementation type with service type and service key<br/>
		/// 根据服务类型和服务键注册实现类型<br/>
		/// </summary>
		public void Register<TService, TImplementation>(ReuseType reuseType, object serviceKey) {
			Register(typeof(TService), typeof(TImplementation), reuseType, serviceKey);
		}

		/// <summary>
		/// Register implementation type with service types and service key<br/>
		/// 根据多个服务类型和服务键注册实现类型<br/>
		/// </summary>
		public void RegisterMany(
			IList<Type> serviceTypes, Type implementationType, ReuseType reuseType, object serviceKey) {
			var factory = this.BuildFactory(implementationType, reuseType);
			var factoryData = new ContainerFactoryData(factory, implementationType);
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
			RegisterMany(typeof(TImplementation), reuseType, serviceKey, nonPublicServiceTypes);
		}

		/// <summary>
		/// Register instance with service type and service key<br/>
		/// Reuse type is forced to Singleton<br/>
		/// 根据服务类型和服务键注册实例<br/>
		/// 重用类型强制为单例<br/>
		/// </summary>
		public void RegisterInstance(Type serviceType, object instance, object serviceKey) {
			var factory = this.BuildFactory(() => instance, ReuseType.Singleton);
			var factoryData = new ContainerFactoryData(factory, instance.GetType());
			RegisterFactory(serviceType, factoryData, serviceKey);
		}

		/// <summary>
		/// Register instance with service type and service key<br/>
		/// Reuse type is forced to Singleton<br/>
		/// 根据服务类型和服务键注册实例<br/>
		/// 重用类型强制为单例<br/>
		/// </summary>
		public void RegisterInstance<TService>(TService instance, object serviceKey) {
			RegisterInstance(typeof(TService), instance, serviceKey);
		}

		/// <summary>
		/// Register delegate with service type and service key<br/>
		/// 根据服务类型和服务键注册工厂函数<br/>
		/// </summary>
		public void RegisterDelegate(
			Type serviceType, Func<object> factory, ReuseType reuseType, object serviceKey) {
			factory = this.BuildFactory(factory, reuseType);
			// We can't known what type will be returned, the hint type will be service type
			var factoryData = new ContainerFactoryData(factory, serviceType);
			RegisterFactory(serviceType, factoryData, serviceKey);
		}

		/// <summary>
		/// Register delegate with service type and service key<br/>
		/// 根据服务类型和服务键注册工厂函数<br/>
		/// </summary>
		public void RegisterDelegate<TService>(
			Func<TService> factory, ReuseType reuseType, object serviceKey) {
			RegisterDelegate(typeof(TService), () => factory(), reuseType, serviceKey);
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
				FactoriesLock.ExitWriteLock();
			}
			GC.Collect();
		}

		/// <summary>
		/// Update factories cache for service type and return newest data<br/>
		/// 根据服务类型更新工厂函数的缓存并返回最新的数据<br/>
		/// </summary>
		internal ContainerFactoriesCacheData UpdateFactoriesCache<TService>() {
			var key = Pair.Create(typeof(TService), (object)null);
			var factoriesCopy = new List<Func<object>>();
			FactoriesLock.EnterReadLock();
			try {
				var factories = Factories.GetOrDefault(key);
				if (factories != null) {
					factoriesCopy.Capacity = factories.Count;
					factoriesCopy.AddRange(factories.Select(f => f.Factory));
				}
				var data = new ContainerFactoriesCacheData(this, factoriesCopy);
				ContainerFactoriesCache<TService>.Data = data;
				return data;
			} finally {
				FactoriesLock.ExitReadLock();
			}
		}

		/// <summary>
		/// Resolve service with type and key<br/>
		/// Throw exception or return default value if not found, dependent on ifUnresolved<br/>
		/// 根据服务类型和服务键获取实例<br/>
		/// 找不到时根据ifUnresolved参数抛出例外或者返回默认值<br/>
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
					factory = factories[0].Factory;
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
					return (TService)data.SingleFactory();
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
			FactoriesLock.EnterReadLock();
			try {
				// Copy factories
				var factories = Factories.GetOrDefault(key);
				if (factories != null) {
					factoriesCopy.Capacity = factories.Count;
					factoriesCopy.AddRange(factories.Select(f => f.Factory));
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
				foreach (var factory in data.Factories) {
					yield return (TService)factory();
				}
			} else {
				// Use default method
				foreach (var instance in ResolveMany(typeof(TService), serviceKey)) {
					yield return (TService)instance;
				}
			}
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
			FactoriesLock.EnterReadLock();
			try {
				foreach (var pair in Factories) {
					clone.Factories[pair.Key] = pair.Value.ToList();
				}
				clone.RegisterSelf(); // replace self instances
				clone.Revision = Revision; // inherit revision
			} finally {
				FactoriesLock.ExitReadLock();
			}
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
