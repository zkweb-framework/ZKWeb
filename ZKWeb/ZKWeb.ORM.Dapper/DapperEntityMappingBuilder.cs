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
	/// 定义一个实体的映射<br/>
	/// </summary>
	/// <typeparam name="T">Entity type</typeparam>
	public class DapperEntityMappingBuilder<T> :
		DommelEntityMap<T>,
		IEntityMappingBuilder<T>,
		IDapperEntityMapping
		where T : class, IEntity {
		/// <summary>
		/// Entity type<br/>
		/// 实体类型<br/>
		/// </summary>
		public Type EntityType { get { return typeof(T); } }
		/// <summary>
		/// Id member<br/>
		/// Id成员<br/>
		/// </summary>
		public MemberInfo IdMember { get { return idMember; } }
#pragma warning disable CS1591
		protected MemberInfo idMember;
#pragma warning restore CS1591
		/// <summary>
		/// Ordinary members<br/>
		/// 普通成员列表<br/>
		/// </summary>
		public IEnumerable<MemberInfo> OrdinaryMembers { get { return ordinaryMembers; } }
#pragma warning disable CS1591
		protected IList<MemberInfo> ordinaryMembers;
#pragma warning restore CS1591
		/// <summary>
		/// ORM name<br/>
		/// ORM名称<br/>
		/// </summary>
		public string ORM { get { return DapperDatabaseContext.ConstORM; } }
		/// <summary>
		/// Custom table name<br/>
		/// 自定义表名<br/>
		/// </summary>
		protected string CustomTableName { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public DapperEntityMappingBuilder(
			IEnumerable<IDatabaseInitializeHandler> handlers,
			IEnumerable<IEntityMappingProvider> providers) {
			idMember = null;
			ordinaryMembers = new List<MemberInfo>();
			// Configure with registered providers
			foreach (IEntityMappingProvider<T> provider in providers) {
				provider.Configure(this);
			}
			// Ignore members that not mapped by this builder
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
			// Set table name with registered handlers
			var tableName = CustomTableName ?? typeof(T).Name;
			foreach (var handler in handlers) {
				handler.ConvertTableName(ref tableName);
			}
			base.ToTable(tableName);
		}

		/// <summary>
		/// Specify the custom table name<br/>
		/// 指定自定义表名<br/>
		/// </summary>
		void IEntityMappingBuilder<T>.TableName(string tableName) {
			CustomTableName = tableName;
		}

		/// <summary>
		/// Specify the primary key for this entity<br/>
		/// 指定实体的主键<br/>
		/// </summary>
		public void Id<TPrimaryKey>(
			Expression<Func<T, TPrimaryKey>> memberExpression,
			EntityMappingOptions options) {
			options = options ?? new EntityMappingOptions();
			var idMap = base.Map(Expression.Lambda<Func<T, object>>(
				Expression.Convert(memberExpression.Body, typeof(object)),
				memberExpression.Parameters));
			idMap = idMap.IsKey();
			if (typeof(TPrimaryKey) == typeof(int) || typeof(TPrimaryKey) == typeof(long)) {
				// Recognize integer primary key as auto increment
				// For now there non explicit option for this
				idMap = idMap.IsIdentity();
			}
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
		/// 创建成员映射<br/>
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
		/// 创建到其他实体的映射, 这是多对一的关系<br/>
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
		/// 创建到实体集合的映射, 这是一对多的关系<br/>
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
		/// 创建到实体集合的映射, 这是多对多的关系<br/>
		/// </summary>
		public void HasManyToMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options = null)
			where TChild : class {
			// log error only, some functions may not work
			var logManager = Application.Ioc.Resolve<LogManager>();
			logManager.LogError($"HasManyToMany is unsupported with dapper, expression: {memberExpression}");
		}
	}
}
