using Dapper.FluentMap;
using Dapper.FluentMap.Dommel;
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
			Mappings = new ConcurrentDictionary<Type, IDapperEntityMapping>();
			// Build entity mappings
			var providers = Application.Ioc.ResolveMany<IEntityMappingProvider>();
			var groupedProviders = providers.GroupBy(p =>
				ReflectionUtils.GetGenericArguments(
				p.GetType(), typeof(IEntityMappingProvider<>))[0]);
			foreach (var group in groupedProviders) {
				var builder = (IDapperEntityMapping)Activator.CreateInstance(
					typeof(DapperEntityMappingBuilder<>).MakeGenericType(group.Key));
				builder.IgnoreExtraMembers();
				Mappings[group.Key] = builder;
			}
			// Setup dommel mappings
			FluentMapper.Initialize(config => {
				var addMap = config.GetType().FastGetMethod(nameof(config.AddMap));
				foreach (var mapping in Mappings) {
					addMap.MakeGenericMethod(mapping.Key).FastInvoke(config, mapping.Value);
				}
				config.ForDommel();
			});
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
