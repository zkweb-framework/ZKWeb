using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Static IoC container factories cache<br/>
	/// 静态的IoC容器工厂函数缓存<br/>
	/// </summary>
	/// <seealso cref="Container"/>
	/// <seealso cref="ContainerFactoriesCacheData{TService}"/>
	/// <seealso cref="ContainerFactoriesCache{TService}"/>
	public static class ContainerFactoriesCache {
		/// <summary>
		/// Set to enable static factories cache<br/>
		/// 是否启用静态工厂函数缓存<br/>
		/// </summary>
		public static bool Enabled = true;
	}

	/// <summary>
	/// IoC container factories cache data<br/>
	/// IoC容器工厂函数缓存的数据<br/>
	/// </summary>
	/// <seealso cref="Container"/>
	/// <seealso cref="ContainerFactoriesCache"/>
	/// <seealso cref="ContainerFactoriesCache{TService}"/>
	internal class ContainerFactoriesCacheData<TService> {
		/// <summary>
		/// Matched container instance<br/>
		/// 容器的实例<br/>
		/// </summary>
		internal IContainer Container = null;
		/// <summary>
		/// Matched container revision<br/>
		/// 容器的版本<br/>
		/// </summary>
		internal int Revision = 0;
		/// <summary>
		/// Factories for given services<br/>
		/// Service key should be null<br/>
		/// 服务的工厂函数列表<br/>
		/// 服务键应该为null<br/>
		/// </summary>
		internal List<Func<TService>> Factories = null;
		/// <summary>
		/// Single factory<br/>
		/// Available when Factories only contains one element<br/>
		/// 单个工厂函数<br/>
		/// 只在工厂函数列表只包含一个元素时有值<br/>
		/// </summary>
		internal Func<TService> SingleFactory = null;

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ContainerFactoriesCacheData(
			Container container, List<Func<TService>> factories) {
			Container = container;
			Revision = container.Revision;
			Factories = factories;
			if (factories.Count == 1) {
				SingleFactory = factories[0];
			}
		}

		/// <summary>
		/// Check if container matched this cache data<br/>
		/// 检查容器是否匹配这个缓存数据<br/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool IsMatched(Container container) {
			return (
				object.ReferenceEquals(Container, container) &&
				Revision == container.Revision);
		}
	}

	/// <summary>
	/// Static IoC container factories cache<br/>
	/// 静态IoC容器的工厂函数缓存<br/>
	/// </summary>
	/// <typeparam name="TService">Service Type</typeparam>
	/// <seealso cref="Container"/>
	/// <seealso cref="ContainerFactoriesCache"/>
	/// <seealso cref="ContainerFactoriesCacheData{TService}"/>
	internal static class ContainerFactoriesCache<TService> {
		/// <summary>
		/// Cache data<br/>
		/// 缓存数据<br/>
		/// </summary>
		internal static ContainerFactoriesCacheData<TService> Data = null;
	}
}
