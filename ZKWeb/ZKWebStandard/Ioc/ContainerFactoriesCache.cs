using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Static ioc container factories cache<br/>
	/// <br/>
	/// </summary>
	public static class ContainerFactoriesCache {
		/// <summary>
		/// Set to enable static factories cache<br/>
		/// <br/>
		/// </summary>
		public static bool Enabled = true;
	}

	/// <summary>
	/// IoC container factories cache data<br/>
	/// <br/>
	/// </summary>
	internal class ContainerFactoriesCacheData {
		/// <summary>
		/// Matched container instance<br/>
		/// <br/>
		/// </summary>
		internal IContainer Container = null;
		/// <summary>
		/// Matched container revision<br/>
		/// <br/>
		/// </summary>
		internal int Revision = 0;
		/// <summary>
		/// Factories for given services<br/>
		/// Service key should be null<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		internal List<Func<object>> Factories = null;
		/// <summary>
		/// Single factory<br/>
		/// Available when Factories only contains one element<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		internal Func<object> SingleFactory = null;

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ContainerFactoriesCacheData(
			Container container, List<Func<object>> factories) {
			Container = container;
			Revision = container.Revision;
			Factories = factories;
			if (factories.Count == 1) {
				SingleFactory = factories[0];
			}
		}

		/// <summary>
		/// Check if container matched this cache data<br/>
		/// <br/>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool IsMatched(Container container) {
			return (
				object.ReferenceEquals(Container, container) &&
				Revision == container.Revision);
		}
	}

	/// <summary>
	/// Static ioc container factories cache<br/>
	/// <br/>
	/// </summary>
	/// <typeparam name="TService">Service Type</typeparam>
	internal static class ContainerFactoriesCache<TService> {
		/// <summary>
		/// Cache data<br/>
		/// <br/>
		/// </summary>
		internal static ContainerFactoriesCacheData Data = null;
	}
}
