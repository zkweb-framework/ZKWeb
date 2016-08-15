using Dapper.Contrib.Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using ZKWeb.Database;
using ZKWebStandard.Extensions;
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
				Mappings[group.Key] = builder;
			}
			// Set table name mapper
			var previousMapper = SqlMapperExtensions.TableNameMapper;
			SqlMapperExtensions.TableNameMapper = type => {
				var mapping = Mappings.GetOrDefault(type);
				if (mapping != null) {
					return mapping.TableName;
				}
				return previousMapper?.Invoke(type);
			};
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
