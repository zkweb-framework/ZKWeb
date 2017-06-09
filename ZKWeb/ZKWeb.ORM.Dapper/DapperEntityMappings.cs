using Dapper.FluentMap;
using Dapper.FluentMap.Dommel;
using Dommel;
using System;
using System.Collections.Concurrent;
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
		public DapperEntityMappings() {
			Mappings = new ConcurrentDictionary<Type, IDapperEntityMapping>();
			// Build entity mappings
			var providers = Application.Ioc.ResolveMany<IEntityMappingProvider>();
			var entityTypes = providers
				.Select(p => ReflectionUtils.GetGenericArguments(
					p.GetType(), typeof(IEntityMappingProvider<>))[0])
				.Distinct().ToList();
			foreach (var entityType in entityTypes) {
				var builder = Activator.CreateInstance(
					typeof(DapperEntityMappingBuilder<>).MakeGenericType(entityType));
				Mappings[entityType] = (IDapperEntityMapping)builder;
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
