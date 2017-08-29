using Dapper.FluentMap;
using Dapper.FluentMap.Dommel;
using Dommel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.FastReflection;
using System.Linq;
using ZKWeb.Database;
using ZKWeb.ORM.Dapper.PropertyResolver;
using ZKWebStandard.Utils;

namespace ZKWeb.ORM.Dapper {
	/// <summary>
	/// Dapper entity mappings<br/>
	/// Dapper的实体映射集合<br/>
	/// </summary>
	public class DapperEntityMappings {
		/// <summary>
		/// Type to mapping definition<br/>
		/// 类型到映射的定义<br/>
		/// </summary>
		public ConcurrentDictionary<Type, IDapperEntityMapping> Mappings { get; protected set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public DapperEntityMappings(
			IEnumerable<IDatabaseInitializeHandler> handlers,
			IEnumerable<IEntityMappingProvider> providers) {
			Mappings = new ConcurrentDictionary<Type, IDapperEntityMapping>();
			// Build entity mappings
			var entityProviders = providers
				.GroupBy(p => ReflectionUtils.GetGenericArguments(
					p.GetType(), typeof(IEntityMappingProvider<>))[0])
				.ToList();
			foreach (var group in entityProviders) {
				var builder = Activator.CreateInstance(
					typeof(DapperEntityMappingBuilder<>).MakeGenericType(group.Key),
					handlers, group.AsEnumerable());
				Mappings[group.Key] = (IDapperEntityMapping)builder;
			}
			// Setup dommel mappings
			FluentMapper.Initialize(config => {
				var addMap = config.GetType().FastGetMethod(nameof(config.AddMap));
				foreach (var mapping in Mappings) {
					addMap.MakeGenericMethod(mapping.Key).FastInvoke(config, mapping.Value);
				}
				config.ForDommel();
				DommelMapper.SetPropertyResolver(new ZKWebPropertyResolver());
			});
		}

		/// <summary>
		/// Get mapping for entity type<br/>
		/// 获取实体类型对应的映射<br/>
		/// </summary>
		/// <param name="type">Entity type</param>
		/// <returns></returns>
		public IDapperEntityMapping GetMapping(Type type) {
			return Mappings[type];
		}
	}
}
