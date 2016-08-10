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
	/// Ioc容器
	/// 容器特性
	/// - 不支持构造函数注入，添加的类型必须带无参数的构造函数
	/// - 不支持构造函数注入的原因
	///   - 减少注册时间
	///   - 不需要编写大量多余的代码
	/// 性能测试
	/// - Register Transient: 1000000/2.3s (DryIoc: 6.1s)
	/// - Register Singleton: 1000000/2.6s (DryIoc: 5.2s)
	/// - Resolve Transient: 10000000/3.2s (DryIoc: 0.54s)
	/// - Resolve Singleton: 10000000/3.2s (DryIoc: 0.43s)
	/// - ResolveMany Transient: 10000000/11.8s (DryIoc: 14.7s)
	/// - ResolveMany Singleton: 10000000/11.1s (DryIoc: 12.9s)
	/// </summary>
	public class Container : IContainer {
		/// <summary>
		/// 生成函数的集合
		/// { (类型, 关联键): [生成函数, ...], ... }
		/// </summary>
		protected IDictionary<Pair<Type, object>, IList<Func<object>>> Factories { get; set; }
		/// <summary>
		/// 生成函数的集合的线程锁
		/// </summary>
		protected ReaderWriterLockSlim FactoriesLock { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		public Container() {
			Factories = new Dictionary<Pair<Type, object>, IList<Func<object>>>();
			FactoriesLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
		}

		/// <summary>
		/// 从原始的生成函数和重用策略构建生成函数
		/// </summary>
		protected static Func<object> BuildFactory(Func<object> originalFactor, ReuseType reuseType) {
			if (reuseType == ReuseType.Transient) {
				// 即时模式
				return originalFactor;
			} else if (reuseType == ReuseType.Singleton) {
				// 单例模式
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
						value = originalFactor();
						return value;
					}
				};
			} else {
				throw new NotSupportedException(string.Format("unsupported reuse type {0}", reuseType));
			}
		}

		/// <summary>
		/// 类型的默认生成函数的缓存
		/// </summary>
		protected readonly static ConcurrentDictionary<Type, Func<object>> TypeFactorsCache =
			new ConcurrentDictionary<Type, Func<object>>();

		/// <summary>
		/// 从类型和重用策略构建生成函数
		/// </summary>
		protected static Func<object> BuildFactor(Type type, ReuseType reuseType) {
			var typeFactor = TypeFactorsCache.GetOrAdd(type,
				t => Expression.Lambda<Func<object>>(Expression.New(t)).Compile());
			return BuildFactory(typeFactor, reuseType);
		}

		/// <summary>
		/// 获取类型的自身类型和继承的类型和实现的接口列表
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
		/// 获取类型的自身类型和继承的类型和实现的接口列表
		/// 部分系统类型和接口会被忽略
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
		/// 注册生成函数到服务类型，并关联指定的键
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
		/// 注册生成函数到多个服务类型，并关联指定的键
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
		/// 注册实现类型到服务类型，并关联指定的键
		/// </summary>
		public void Register(
			Type serviceType, Type implementationType, ReuseType reuseType, object serviceKey) {
			var factory = BuildFactor(implementationType, reuseType);
			RegisterFactory(serviceType, factory, serviceKey);
		}

		/// <summary>
		/// 注册实现类型到服务类型，并关联指定的键
		/// </summary>
		public void Register<TService, TImplementation>(ReuseType reuseType, object serviceKey) {
			Register(typeof(TService), typeof(TImplementation), reuseType, serviceKey);
		}

		/// <summary>
		/// 注册实现类型到多个服务类型，并关联指定的键
		/// </summary>
		public void RegisterMany(
			IList<Type> serviceTypes, Type implementationType, ReuseType reuseType, object serviceKey) {
			var factory = BuildFactor(implementationType, reuseType);
			RegisterFactoryMany(serviceTypes, factory, serviceKey);
		}

		/// <summary>
		/// 注册实现类型到它自身和继承的类型和实现的接口，并关联指定的键
		/// 部分系统类型和接口会被忽略
		/// </summary>
		public void RegisterMany(Type implementationType,
			ReuseType reuseType, object serviceKey, bool nonPublicServiceTypes) {
			var serviceTypes = GetImplementedServiceTypes(
				implementationType, nonPublicServiceTypes).ToList();
			RegisterMany(serviceTypes, implementationType, reuseType, serviceKey);
		}

		/// <summary>
		/// 注册实现类型到它自身和继承的类型，并关联指定的键
		/// 部分系统自带的类型会被忽略
		/// </summary>
		public void RegisterMany<TImplementation>(
			ReuseType reuseType, object serviceKey, bool nonPublicServiceTypes) {
			RegisterMany(typeof(TImplementation), reuseType, serviceKey, nonPublicServiceTypes);
		}

		/// <summary>
		/// 注册实例到服务类型，并关联指定的键
		/// 默认是单例模式
		/// </summary>
		public void RegisterInstance(Type serviceType, object instance, object serviceKey) {
			var factory = BuildFactory(() => instance, ReuseType.Singleton);
			RegisterFactory(serviceType, factory, serviceKey);
		}

		/// <summary>
		/// 注册实例到服务类型，并关联指定的键
		/// 默认是单例模式
		/// </summary>
		public void RegisterInstance<TService>(TService instance, object serviceKey) {
			RegisterInstance(typeof(TService), instance, serviceKey);
		}

		/// <summary>
		/// 注册生成函数到服务类型，并关联指定的键
		/// </summary>
		public void RegisterDelegate(
			Type serviceType, Func<object> factory, ReuseType reuseType, object serviceKey) {
			factory = BuildFactory(factory, reuseType);
			RegisterFactory(serviceType, factory, serviceKey);
		}

		/// <summary>
		/// 注册生成函数到服务类型，并关联指定的键
		/// </summary>
		public void RegisterDelegate<TService>(
			Func<TService> factory, ReuseType reuseType, object serviceKey) {
			RegisterDelegate(typeof(TService), () => factory(), reuseType, serviceKey);
		}

		/// <summary>
		/// 注册使用属性导出的类型
		/// </summary>
		public void RegisterExports(IEnumerable<Type> types) {
			foreach (var type in types) {
				var typeInfo = type.GetTypeInfo();
				var exportManyAttribute = typeInfo.GetAttribute<ExportManyAttribute>();
				if (exportManyAttribute == null) {
					continue;
				}
				// 按ExportMany属性注册
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
		/// 取消注册到服务类型并关联了指定的键的所有生成函数
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
		/// 取消注册到服务类型并关联了指定的键的所有生成函数
		/// </summary>
		public void Unregister<TService>(object serviceKey) {
			Unregister(typeof(TService), serviceKey);
		}

		/// <summary>
		/// 取消所有注册的生成函数
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
		/// 获取注册到服务类型并关联了指定键的单个实例
		/// 没有注册或注册了多个时按无法解决时的策略处理
		/// </summary>
		public object Resolve(Type serviceType, IfUnresolved ifUnresolved, object serviceKey) {
			var key = Pair.Create(serviceType, serviceKey);
			Func<object> factory = null;
			long factoriesCount = 0;
			FactoriesLock.EnterReadLock();
			try {
				// 获取生成函数和生成函数的数量
				// 只有在注册了单个生成函数时获取成功
				var factories = Factories.GetOrDefault(key);
				factoriesCount = factories?.Count ?? 0;
				if (factoriesCount == 1) {
					factory = factories[0];
				}
			} finally {
				FactoriesLock.ExitReadLock();
			}
			if (factory != null) {
				// 获取成功
				return factory();
			} else if (ifUnresolved == IfUnresolved.Throw) {
				// 抛出例外
				var messageFormat = (factoriesCount <= 0) ?
					"no factory registered to type {0} and service key {1}" :
					"more than one factory registered to type {0} and service key {1}";
				throw new KeyNotFoundException(string.Format(messageFormat, serviceType, serviceKey));
			} else if (ifUnresolved == IfUnresolved.ReturnDefault) {
				// 返回默认值
				return serviceType.GetTypeInfo().IsValueType ?
					Activator.CreateInstance(serviceType) : null;
			} else {
				throw new NotSupportedException(string.Format(
					"unsupported ifUnresolved type {0}", ifUnresolved));
			}
		}

		/// <summary>
		/// 获取注册到服务类型并关联了指定键的单个实例
		/// 没有注册或注册了多个时按无法解决时的策略处理
		/// </summary>
		public TService Resolve<TService>(IfUnresolved ifUnresolved, object serviceKey) {
			return (TService)Resolve(typeof(TService), ifUnresolved, serviceKey);
		}

		/// <summary>
		/// 获取注册到服务类型并关联了指定键的单个或多个实例
		/// 没有注册时返回空列表
		/// </summary>
		public IEnumerable<object> ResolveMany(Type serviceType, object serviceKey) {
			var key = Pair.Create(serviceType, serviceKey);
			var factorsCopy = new List<Func<object>>();
			FactoriesLock.EnterReadLock();
			try {
				// 复制生成函数列表
				var factors = Factories.GetOrDefault(key);
				if (factors != null) {
					factorsCopy.Capacity = factors.Count;
					factorsCopy.AddRange(factors);
				}
			} finally {
				FactoriesLock.ExitReadLock();
			}
			// 生成实例并逐个返回
			foreach (var factory in factorsCopy) {
				yield return factory();
			}
		}

		/// <summary>
		/// 获取注册到服务类型并关联了指定键的单个或多个实例
		/// 没有注册时返回空列表
		/// </summary>
		public IEnumerable<TService> ResolveMany<TService>(object serviceKey) {
			foreach (var instance in ResolveMany(typeof(TService), serviceKey)) {
				yield return (TService)instance;
			}
		}

		/// <summary>
		/// 克隆容器
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
		/// 释放容器
		/// </summary>
		public void Dispose() {
			GC.Collect();
		}
	}
}
