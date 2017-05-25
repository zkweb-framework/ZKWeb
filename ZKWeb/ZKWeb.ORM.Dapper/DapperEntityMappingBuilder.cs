using Dapper.FluentMap.Dommel.Mapping;
using System;
using System.Collections.Generic;
using System.FastReflection;
using System.Linq.Expressions;
using System.Reflection;
using ZKWeb.Database;
using ZKWeb.Logging;
using ZKWeb.ORM.Dapper.TypeHandlers;
using ZKWebStandard.Extensions;

namespace ZKWeb.ORM.Dapper {
	/// <summary>
	/// Defines a mapping for an entity<br/>
	/// <br/>
	/// </summary>
	/// <typeparam name="T">Entity type</typeparam>
	internal class DapperEntityMappingBuilder<T> :
		DommelEntityMap<T>,
		IEntityMappingBuilder<T>,
		IDapperEntityMapping
		where T : class, IEntity {
		public Type EntityType { get { return typeof(T); } }
		public MemberInfo IdMember { get { return idMember; } }
		private MemberInfo idMember;
		public IEnumerable<MemberInfo> OrdinaryMembers { get { return ordinaryMembers; } }
		private IList<MemberInfo> ordinaryMembers;
		public string ORM { get { return DapperDatabaseContext.ConstORM; } }
		public object NativeBuilder { get { return this; } set { } }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		public DapperEntityMappingBuilder() {
			idMember = null;
			ordinaryMembers = new List<MemberInfo>();
			// Configure with registered providers
			var providers = Application.Ioc.ResolveMany<IEntityMappingProvider<T>>();
			foreach (var provider in providers) {
				provider.Configure(this);
			}
			// Set table name with registered handlers
			var tableName = typeof(T).Name;
			var handlers = Application.Ioc.ResolveMany<IDatabaseInitializeHandler>();
			handlers.ForEach(h => h.ConvertTableName(ref tableName));
			base.ToTable(tableName);
		}

		/// <summary>
		/// Specify the primary key for this entity<br/>
		/// <br/>
		/// </summary>
		public void Id<TPrimaryKey>(
			Expression<Func<T, TPrimaryKey>> memberExpression,
			EntityMappingOptions options) {
			options = options ?? new EntityMappingOptions();
			var idMap = base.Map(Expression.Lambda<Func<T, object>>(
				Expression.Convert(memberExpression.Body, typeof(object)),
				memberExpression.Parameters));
			idMap = idMap.IsKey();
			if (!string.IsNullOrEmpty(options.Column)) {
				idMap = idMap.ToColumn(options.Column);
			}
			if (options.WithSerialization == true) {
				TypeHandlerRegistrator.RegisterJsonSerializedType(typeof(TPrimaryKey));
			}
			if (typeof(TPrimaryKey) == typeof(Guid)) {
				TypeHandlerRegistrator.Register(typeof(Guid), new GuidTypeHandler());
			}
			idMember = ((MemberExpression)memberExpression.Body).Member;
		}

		/// <summary>
		/// Create a member mapping<br/>
		/// <br/>
		/// </summary>
		public void Map<TMember>(
			Expression<Func<T, TMember>> memberExpression,
			EntityMappingOptions options) {
			options = options ?? new EntityMappingOptions();
			var memberMap = base.Map(Expression.Lambda<Func<T, object>>(
				Expression.Convert(memberExpression.Body, typeof(object)),
				memberExpression.Parameters));
			if (!string.IsNullOrEmpty(options.Column)) {
				memberMap = memberMap.ToColumn(options.Column);
			}
			if (options.WithSerialization ?? false) {
				TypeHandlerRegistrator.RegisterJsonSerializedType(typeof(TMember));
			}
			ordinaryMembers.Add(((MemberExpression)memberExpression.Body).Member);
		}

		/// <summary>
		/// Create a reference to another entity, this is a many-to-one relationship.<br/>
		/// <br/>
		/// </summary>
		public void References<TOther>(
			Expression<Func<T, TOther>> memberExpression,
			EntityMappingOptions options)
			where TOther : class {
			// log error only, some functions may not work
			var logManager = Application.Ioc.Resolve<LogManager>();
			logManager.LogError($"References is unsupported with dapper, expression: {memberExpression}");
		}

		/// <summary>
		/// Maps a collection of entities as a one-to-many relationship.<br/>
		/// <br/>
		/// </summary>
		public void HasMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options)
			where TChild : class {
			// log error only, some functions may not work
			var logManager = Application.Ioc.Resolve<LogManager>();
			logManager.LogError($"HasMany is unsupported with dapper, expression: {memberExpression}");
		}

		/// <summary>
		/// Maps a collection of entities as a many-to-many relationship.<br/>
		/// <br/>
		/// </summary>
		public void HasManyToMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options = null)
			where TChild : class {
			// log error only, some functions may not work
			var logManager = Application.Ioc.Resolve<LogManager>();
			logManager.LogError($"HasManyToMany is unsupported with dapper, expression: {memberExpression}");
		}

		/// <summary>
		/// Ignore members that not mapped by this builder<br/>
		/// <br/>
		/// </summary>
		public void IgnoreExtraMembers() {
			var existMembers = new HashSet<MemberInfo>();
			existMembers.Add(idMember);
			existMembers.AddRange(ordinaryMembers);
			foreach (var property in typeof(T).FastGetProperties()) {
				if (existMembers.Contains(property)) {
					continue;
				}
				var parameter = Expression.Parameter(typeof(T));
				var memberExpression = Expression.Lambda<Func<T, object>>(
					Expression.Convert(Expression.Property(parameter, property), typeof(object)),
					parameter);
				base.Map(memberExpression).Ignore();
			}
		}
	}
}
