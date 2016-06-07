using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZKWeb.Utils.Collections;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Utils.IocContainer {
	/// <summary>
	/// Ioc容器
	/// 容器特性
	/// - 不支持构造函数注入，添加的类型必须带无参数的构造函数
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
		protected IDictionary<Pair<Type, object>, IList<Func<object>>> Factors { get; set; }
		/// <summary>
		/// 生成函数的集合的线程锁
		/// </summary>
		protected ReaderWriterLockSlim FactorsLock { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		public Container() {
			Factors = new Dictionary<Pair<Type, object>, IList<Func<object>>>();
			FactorsLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
		}

		/// <summary>
		/// 从原始的生成函数和重用策略构建生成函数
		/// </summary>
		protected static Func<object> BuildFactor(Func<object> originalFactor, ReuseType reuseType) {
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
			return BuildFactor(typeFactor, reuseType);
		}

		/// <summary>
		/// 获取类型的自身类型和继承的类型和实现的接口列表
		/// </summary>
		protected static IEnumerable<Type> GetImplementedTypes(Type type) {
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
		/// 获取类型的自身类型和继承的类型和实现的接口列表
		/// 部分系统类型和接口会被忽略
		/// </summary>
		protected static IEnumerable<Type> GetImplementedServiceTypes(Type type, bool nonPublicServiceTypes) {
			var mscorlibAssembly = typeof(int).Assembly;
			foreach (var serviceType in GetImplementedTypes(type)) {
				if ((!serviceType.IsNotPublic || nonPublicServiceTypes) &&
					(serviceType.Assembly != mscorlibAssembly)) {
					yield return serviceType;
				}
			}
		}

		/// <summary>
		/// 注册生成函数到服务类型，并关联指定的键
		/// </summary>
		protected void RegisterFactor(Type serviceType, Func<object> factor, object serviceKey) {
			var key = Pair.Create(serviceType, serviceKey);
			FactorsLock.EnterWriteLock();
			try {
				var factors = Factors.GetOrCreate(key, () => new List<Func<object>>());
				factors.Add(factor);
			} finally {
				FactorsLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// 注册生成函数到多个服务类型，并关联指定的键
		/// </summary>
		protected void RegisterFactorMany(IList<Type> serviceTypes, Func<object> factor, object serviceKey) {
			FactorsLock.EnterWriteLock();
			try {
				foreach (var serviceType in serviceTypes) {
					var key = Pair.Create(serviceType, serviceKey);
					var factors = Factors.GetOrCreate(key, () => new List<Func<object>>());
					factors.Add(factor);
				}
			} finally {
				FactorsLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// 注册实现类型到服务类型，并关联指定的键
		/// </summary>
		public void Register(
			Type serviceType, Type implementationType, ReuseType reuseType, object serviceKey) {
			var factor = BuildFactor(implementationType, reuseType);
			RegisterFactor(serviceType, factor, serviceKey);
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
			var factor = BuildFactor(implementationType, reuseType);
			RegisterFactorMany(serviceTypes, factor, serviceKey);
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
			var factor = BuildFactor(() => instance, ReuseType.Singleton);
			RegisterFactor(serviceType, factor, serviceKey);
		}

		/// <summary>
		/// 注册实例到服务类型，并关联指定的键
		/// 默认是单例模式
		/// </summary>
		public void RegisterInstance<TService>(object instance, object serviceKey) {
			RegisterInstance(typeof(TService), instance, serviceKey);
		}

		/// <summary>
		/// 注册生成函数到服务类型，并关联指定的键
		/// </summary>
		public void RegisterDelegate(
			Type serviceType, Func<object> factor, ReuseType reuseType, object serviceKey) {
			factor = BuildFactor(factor, reuseType);
			RegisterFactor(serviceType, factor, serviceKey);
		}

		/// <summary>
		/// 注册生成函数到服务类型，并关联指定的键
		/// </summary>
		public void RegisterDelegate<TService>(
			Func<object> factor, ReuseType reuseType, object serviceKey) {
			RegisterDelegate(typeof(TService), factor, reuseType, serviceKey);
		}

		/// <summary>
		/// 注册使用属性导出的类型
		/// </summary>
		public void RegisterExports(IEnumerable<Type> types) {
			foreach (var type in types) {
				var exportManyAttribute = type.GetAttribute<ExportManyAttributes>();
				var exportAttributes = type.GetAttributes<ExportAttribute>().ToList();
				if (exportManyAttribute == null && exportAttributes.Count <= 0) {
					continue;
				}
				var reuseAttribute = type.GetAttribute<ReuseAttribute>();
				var reuseType = reuseAttribute?.ReuseType ?? default(ReuseType);
				if (exportManyAttribute != null) {
					// 按ExportMany属性注册
					var nonPublic = exportManyAttribute.NonPublic;
					var except = exportManyAttribute.Except;
					var contractKey = exportManyAttribute.ContractKey;
					var serviceTypes = GetImplementedServiceTypes(type, nonPublic);
					if (except != null && except.Any()) {
						serviceTypes = serviceTypes.Where(t => !except.Contains(t));
					}
					RegisterMany(serviceTypes.ToList(), type, reuseType, contractKey);
				} else if (exportAttributes.Count > 0) {
					// 按Export属性注册，已经有ExportMany时不需要再看这个属性
					foreach (var exportAttribute in exportAttributes) {
						Register(exportAttribute.ContractType,
							type, reuseType, exportAttribute.ContractName);
					}
				}
			}
		}

		/// <summary>
		/// 取消注册到服务类型并关联了指定的键的所有生成函数
		/// </summary>
		public void Unregister(Type serviceType, object serviceKey) {
			var key = Pair.Create(serviceType, serviceKey);
			FactorsLock.EnterWriteLock();
			try {
				Factors.Remove(key);
			} finally {
				FactorsLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// 取消注册到服务类型并关联了指定的键的所有生成函数
		/// </summary>
		public void Unregister<TService>(object serviceKey) {
			Unregister(typeof(TService), serviceKey);
		}

		/// <summary>
		/// 获取注册到服务类型并关联了指定键的单个实例
		/// 没有注册或注册了多个时按无法解决时的策略处理
		/// </summary>
		public object Resolve(Type serviceType, IfUnresolved ifUnresolved, object serviceKey) {
			var key = Pair.Create(serviceType, serviceKey);
			Func<object> factor = null;
			long factorsCount = 0;
			FactorsLock.EnterReadLock();
			try {
				// 获取生成函数和生成函数的数量
				// 只有在注册了单个生成函数时获取成功
				var factors = Factors.GetOrDefault(key);
				factorsCount = factors?.Count ?? 0;
				if (factorsCount == 1) {
					factor = factors[0];
				}
			} finally {
				FactorsLock.ExitReadLock();
			}
			if (factor != null) {
				// 获取成功
				return factor();
			} else if (ifUnresolved == IfUnresolved.Throw) {
				// 抛出例外
				var messageFormat = (factorsCount <= 0) ?
					"no factor registered to type {0} and service key {1}" :
					"more than one factor registered to type {0} and service key {1}";
				throw new KeyNotFoundException(string.Format(messageFormat, serviceType, serviceKey));
			} else if (ifUnresolved == IfUnresolved.ReturnDefault) {
				// 返回默认值
				return serviceType.IsValueType ? Activator.CreateInstance(serviceType) : null;
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
			FactorsLock.EnterReadLock();
			try {
				// 复制生成函数列表
				var factors = Factors.GetOrDefault(key);
				if (factors != null) {
					factorsCopy.Capacity = factors.Count;
					factorsCopy.AddRange(factors);
				}
			} finally {
				FactorsLock.ExitReadLock();
			}
			// 生成实例并逐个返回
			foreach (var factor in factorsCopy) {
				yield return factor();
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
			FactorsLock.EnterReadLock();
			try {
				foreach (var pair in Factors) {
					clone.Factors[pair.Key] = pair.Value.ToList();
				}
			} finally {
				FactorsLock.ExitReadLock();
			}
			return clone;
		}

		/// <summary>
		/// 释放容器
		/// </summary>
		public void Dispose() { }
	}
}
