using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Ioc.Extensions;

namespace ZKWebStandard.Ioc {
#pragma warning disable S3881 // "IDisposable" should be implemented correctly
	/// <summary>
	/// IoC container<br/>
	/// Support constructor dependency injection<br/>
	/// IoC容器<br/>
	/// 支持构造函数注入<br/>
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
#pragma warning restore S3881 // "IDisposable" should be implemented correctly
		/// <summary>
		/// Factories<br/>
		/// 工厂函数的集合<br/>
		/// { (Service Type, Service Key): [Factory Data] }
		/// </summary>
		protected ConcurrentDictionary<
			Pair<Type, object>,
			ConcurrentQueue<ContainerFactoryData>> Factories { get; set; }
		/// <summary>
		/// Objects needs to call Dispose when scope finished<br/>
		/// 储存当前范围中需要调用Dispose函数的对象的队列<br/>
		/// </summary>
		protected AsyncLocal<ConcurrentQueue<IDisposable>> ScopedDisposeQueue { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public Container() {
			Factories = new ConcurrentDictionary<
				Pair<Type, object>,
				ConcurrentQueue<ContainerFactoryData>>();
			ScopedDisposeQueue = new AsyncLocal<ConcurrentQueue<IDisposable>>();
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
		public static IEnumerable<Type> GetImplementedServiceTypes(
			Type type, bool nonPublicServiceTypes) {
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
			Unregister<IContainer>();
			Unregister<IRegistrator>();
			Unregister<IGenericRegistrator>();
			Unregister<IResolver>();
			Unregister<IGenericResolver>();
			RegisterInstance<IContainer>(this);
			RegisterInstance<IRegistrator>(this);
			RegisterInstance<IGenericRegistrator>(this);
			RegisterInstance<IResolver>(this);
			RegisterInstance<IGenericResolver>(this);
		}

		/// <summary>
		/// Register factory with service type and service key<br/>
		/// 根据服务类型和服务键注册工厂函数<br/>
		/// </summary>
		protected void RegisterFactory(
			Type serviceType, ContainerFactoryData factoryData, object serviceKey) {
			var key = Pair.Create(serviceType, serviceKey);
			var factories = Factories.GetOrAdd(
				key, _ => new ConcurrentQueue<ContainerFactoryData>());
			factories.Enqueue(factoryData);
		}

		/// <summary>
		/// Register factory with service types and service key<br/>
		/// 根据多个服务类型和服务键注册工厂函数<br/>
		/// </summary>
		protected void RegisterFactoryMany(
			IEnumerable<Type> serviceTypes, ContainerFactoryData factoryData, object serviceKey) {
			foreach (var serviceType in serviceTypes) {
				var key = Pair.Create(serviceType, serviceKey);
				var factories = Factories.GetOrAdd(
					key, _ => new ConcurrentQueue<ContainerFactoryData>());
				factories.Enqueue(factoryData);
			}
		}

		/// <summary>
		/// Register implementation type with service type and service key<br/>
		/// 根据服务类型和服务键注册实现类型<br/>
		/// </summary>
		public void Register(
			Type serviceType, Type implementationType,
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null) {
			var factoryData = new ContainerFactoryData(
				ContainerFactoryBuilder.BuildFactory(implementationType),
				reuseType,
				implementationType);
			RegisterFactory(serviceType, factoryData, serviceKey);
		}

		/// <summary>
		/// Register implementation type with service type and service key<br/>
		/// 根据服务类型和服务键注册实现类型<br/>
		/// </summary>
		public void Register<TService, TImplementation>(
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null) {
			Register(typeof(TService), typeof(TImplementation), reuseType, serviceKey);
		}

		/// <summary>
		/// Register implementation type with service types and service key<br/>
		/// 根据多个服务类型和服务键注册实现类型<br/>
		/// </summary>
		public void RegisterMany(
			IList<Type> serviceTypes, Type implementationType,
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null) {
			var factoryData = new ContainerFactoryData(
				ContainerFactoryBuilder.BuildFactory(implementationType),
				reuseType,
				implementationType);
			RegisterFactoryMany(serviceTypes, factoryData, serviceKey);
		}

		/// <summary>
		/// Register implementation type with service types and service key<br/>
		/// Service types are obtain from base types and interfaces<br/>
		/// 根据多个服务类型和服务键注册实现类型<br/>
		/// 服务类型是实现类型的基类和接口<br/>
		/// </summary>
		public void RegisterMany(Type implementationType,
			ReuseType reuseType = ReuseType.Transient,
			object serviceKey = null, bool nonPublicServiceTypes = false) {
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
			ReuseType reuseType = ReuseType.Transient,
			object serviceKey = null, bool nonPublicServiceTypes = false) {
			RegisterMany(typeof(TImplementation), reuseType, serviceKey, nonPublicServiceTypes);
		}

		/// <summary>
		/// Register instance with service type and service key<br/>
		/// Reuse type is forced to Singleton<br/>
		/// 根据服务类型和服务键注册实例<br/>
		/// 重用类型强制为单例<br/>
		/// </summary>
		public void RegisterInstance(Type serviceType, object instance, object serviceKey = null) {
			var implementationType = instance.GetType();
			var factoryData = new ContainerFactoryData(
				(c, s) => instance,
				ReuseType.Singleton,
				implementationType);
			RegisterFactory(serviceType, factoryData, serviceKey);
		}

		/// <summary>
		/// Register instance with service type and service key<br/>
		/// Reuse type is forced to Singleton<br/>
		/// 根据服务类型和服务键注册实例<br/>
		/// 重用类型强制为单例<br/>
		/// </summary>
		public void RegisterInstance<TService>(TService instance, object serviceKey = null) {
			RegisterInstance(typeof(TService), instance, serviceKey);
		}

		/// <summary>
		/// Register delegate with service type and service key<br/>
		/// 根据服务类型和服务键注册工厂函数<br/>
		/// </summary>
		public void RegisterDelegate(
			Type serviceType, ContainerFactoryDelegate factory,
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null) {
			// Since we can't figure out what type will returned from factory,
			// the implementation hint type will be the service type
			var factoryData = new ContainerFactoryData(
				factory,
				reuseType,
				serviceType);
			RegisterFactory(serviceType, factoryData, serviceKey);
		}

		/// <summary>
		/// Register delegate with service type and service key<br/>
		/// 根据服务类型和服务键注册工厂函数<br/>
		/// </summary>
		public void RegisterDelegate(
			Type serviceType, Func<object> factory,
			ReuseType reuseType = ReuseType.Transient, object serviceKey = null) {
			RegisterDelegate(serviceType, (c, s) => factory(), reuseType, serviceKey);
		}

		/// <summary>
		/// Register delegate with service type and service key<br/>
		/// 根据服务类型和服务键注册工厂函数<br/>
		/// </summary>
		public void RegisterDelegate<TService>(
			Func<TService> factory, ReuseType reuseType = ReuseType.Transient, object serviceKey = null) {
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
		public void Unregister(Type serviceType, object serviceKey = null) {
			var key = Pair.Create(serviceType, serviceKey);
			Factories.TryRemove(key, out var _);
		}

		/// <summary>
		/// Unregister all factories with specified service type and service key<br/>
		/// 注销所有注册到指定服务类型和服务键的工厂函数<br/>
		/// </summary>
		public void Unregister<TService>(object serviceKey = null) {
			Unregister(typeof(TService), serviceKey);
		}

		/// <summary>
		/// Unregister factories with specified implementation type and service key<br/>
		/// 按实现类型和服务键注销所有相关的工厂函数<br/>
		/// </summary>
		public void UnregisterImplementation(Type implementationType, object serviceKey = null) {
			var serviceTypes = GetImplementedServiceTypes(implementationType, true);
			foreach (var serviceType in serviceTypes) {
				var key = Pair.Create(serviceType, serviceKey);
				if (Factories.TryGetValue(key, out var factories)) {
					var newFactories = new ConcurrentQueue<ContainerFactoryData>();
					foreach (var factoryData in factories) {
						if (factoryData.ImplementationTypeHint != implementationType) {
							newFactories.Enqueue(factoryData);
						}
					}
					Factories[key] = newFactories;
				}
			}
		}

		/// <summary>
		/// Unregister factories with specified implementation type and service key<br/>
		/// 按实现类型和服务键注册所有相关的工厂函数<br/>
		/// </summary>
		public void UnregisterImplementation<TImplementation>(object serviceKey = null) {
			UnregisterImplementation(typeof(TImplementation), serviceKey);
		}

		/// <summary>
		/// Unregister all factories<br/>
		/// 注销所有工厂函数<br/>
		/// </summary>
		public void UnregisterAll() {
			Factories.Clear();
#pragma warning disable S1215
			GC.Collect();
#pragma warning restore S1215
		}

		/// <summary>
		/// Resolve service with type and key<br/>
		/// Throw exception or return default value if not found, dependent on ifUnresolved<br/>
		/// 根据服务类型和服务键获取实例<br/>
		/// 找不到时根据ifUnresolved参数抛出例外或者返回默认值<br/>
		/// </summary>
		public object Resolve(
			Type serviceType, IfUnresolved ifUnresolved = IfUnresolved.Throw, object serviceKey = null) {
			var key = Pair.Create(serviceType, serviceKey);
			long factoriesCount = 0;
			// Get factories
			// Success unless there zero or more than one factories
			ConcurrentQueue<ContainerFactoryData> factories;
			if (Factories.TryGetValue(key, out factories)) {
				// Use normal factory
				factoriesCount = factories.Count;
				if (factoriesCount == 1 && factories.TryPeek(out var factoryData)) {
					return factoryData.GetInstance(this, serviceType);
				}
			} else if (serviceType.IsGenericType) {
				// Use factory from generic definition
				var baseKey = Pair.Create(serviceType.GetGenericTypeDefinition(), serviceKey);
				if (Factories.TryGetValue(baseKey, out factories)) {
					factoriesCount = factories.Count;
					if (factoriesCount == 1 && factories.TryPeek(out var factoryData)) {
						return factoryData.GetInstance(this, serviceType);
					}
				}
			}
			if (ifUnresolved == IfUnresolved.Throw) {
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
		public TService Resolve<TService>(
			IfUnresolved ifUnresolved = IfUnresolved.Throw, object serviceKey = null) {
			return (TService)(Resolve(typeof(TService), ifUnresolved, serviceKey) ?? default(TService));
		}

		/// <summary>
		/// Resolve services with type and key<br/>
		/// Return empty sequence if no service registered<br/>
		/// 根据服务类型和服务键获取实例列表<br/>
		/// 如果无注册的服务则返回空列表<br/>
		/// </summary>
		public IEnumerable<object> ResolveMany(Type serviceType, object serviceKey = null) {
			var key = Pair.Create(serviceType, serviceKey);
			ConcurrentQueue<ContainerFactoryData> factories;
			if (Factories.TryGetValue(key, out factories)) {
				// Use normal factories
				foreach (var factoryData in factories) {
					yield return factoryData.GetInstance(this, serviceType);
				}
			} else if (serviceType.IsGenericType) {
				// Use factories from generic definition
				var baseKey = Pair.Create(serviceType.GetGenericTypeDefinition(), serviceKey);
				if (Factories.TryGetValue(baseKey, out factories)) {
					foreach (var factoryData in factories) {
						yield return factoryData.GetInstance(this, serviceType);
					}
				}
			}
		}

		/// <summary>
		/// Resolve services with type and key<br/>
		/// Return empty sequence if no service registered<br/>
		/// 根据服务类型和服务键获取服务列表<br/>
		/// 如果无注册的服务则返回空列表<br/>
		/// </summary>
		public IEnumerable<TService> ResolveMany<TService>(object serviceKey = null) {
			foreach (var instance in ResolveMany(typeof(TService), serviceKey)) {
				yield return (TService)instance;
			}
		}

		/// <summary>
		/// 根据服务类型和服务键获取工厂函数<br/>
		/// 如果无注册的服务则返回空列表<br/>
		/// </summary>
		public IEnumerable<ContainerFactoryData> ResolveFactories(
			Type serviceType, object serviceKey = null) {
			var key = Pair.Create(serviceType, serviceKey);
			if (Factories.TryGetValue(key, out var factories)) {
				return factories;
			} else {
				return Enumerable.Empty<ContainerFactoryData>();
			}
		}

		/// <summary>
		/// 根据服务类型和服务键获取工厂函数<br/>
		/// 如果无注册的服务则返回空列表<br/>
		/// </summary>
		public IEnumerable<ContainerFactoryData> ResolveFactories<TService>(object serviceKey = null) {
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
						queue = new ConcurrentQueue<IDisposable>();
						ScopedDisposeQueue.Value = queue;
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
				var factories = new ConcurrentQueue<ContainerFactoryData>();
				foreach (var factory in pair.Value) {
					factories.Enqueue(factory);
				}
				clone.Factories[pair.Key] = factories;
			}
			clone.RegisterSelf(); // replace self instances
			return clone;
		}

		/// <summary>
		/// Dispose container<br/>
		/// 清理容器的资源<br/>
		/// </summary>
		public void Dispose() {
#pragma warning disable S1215
			GC.Collect();
#pragma warning restore S1215
		}
	}
}
