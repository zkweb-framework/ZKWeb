using System;
using System.Collections.Generic;

namespace ZKWebStandard.Ioc {
	/// <summary>
	/// Static ioc container factories cache
	/// </summary>
	public static class ContainerFactoriesCache {
		/// <summary>
		/// Set to enable static factories cache
		/// </summary>
		public static bool Enabled = true;
	}

	/// <summary>
	/// Static ioc container factories cache
	/// </summary>
	/// <typeparam name="TService">Service Type</typeparam>
	internal static class ContainerFactoriesCache<TService> {
		/// <summary>
		/// Matched container instance
		/// </summary>
		internal static IContainer Container { get; set; }
		/// <summary>
		/// Matched container revision
		/// </summary>
		internal static int Revision { get; set; }
		/// <summary>
		/// Factories for given services
		/// Service key should be null
		/// </summary>
		internal static IList<Func<object>> Factories { get; set; }
	}
}
