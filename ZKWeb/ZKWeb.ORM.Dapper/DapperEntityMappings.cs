using System;
using System.Collections.Concurrent;
using System.FastReflection;
using System.Linq;
using ZKWeb.Database;
using ZKWebStandard.Utils;

namespace ZKWeb.ORM.Dapper {
	/// <summary>
	/// Dapper entity mappings
	/// </summary>
	internal class DapperEntityMappings {
		/// <summary>
		/// Type to mapping definition
		/// </summary>
		private ConcurrentDictionary<Type, IDapperEntityMapping> Mappings { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public DapperEntityMappings() {
			// Build entity mappings
			var providers = Application.Ioc.ResolveMany<IEntityMappingProvider>();
			var groupedProviders = providers.GroupBy(p =>
				ReflectionUtils.GetGenericArguments(
				p.GetType(), typeof(IEntityMappingProvider<>))[0]);
			foreach (var group in groupedProviders) {
				var builder = (IDapperEntityMapping)Activator.CreateInstance(
					typeof(DapperEntityMappingBuilder<>).MakeGenericType(group.Key));
				var configureMethod = typeof(IEntityMappingProvider<>)
					.MakeGenericType(group.Key).FastGetMethod("Configure");
				foreach (var provider in group) {
					configureMethod.FastInvoke(provider, builder);
				}
				Mappings[group.Key] = builder;
			}
		}

		/// <summary>
		/// Get mapping for entity type
		/// </summary>
		/// <param name="type">Entity type</param>
		/// <returns></returns>
		public IDapperEntityMapping GetMapping(Type type) {
			return Mappings[type];
		}
	}
}
