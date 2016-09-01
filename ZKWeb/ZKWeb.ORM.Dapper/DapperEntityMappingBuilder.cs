using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ZKWeb.Database;
using ZKWebStandard.Extensions;

namespace ZKWeb.ORM.Dapper {
	/// <summary>
	/// Defines a mapping for an entity
	/// </summary>
	/// <typeparam name="T">Entity type</typeparam>
	internal class DapperEntityMappingBuilder<T> :
		IEntityMappingBuilder<T>, IDapperEntityMapping
		where T : class, IEntity {
		public Type EntityType { get { return typeof(T); } }
		public string TableName { get { return tableName; } }
		private string tableName;
		public MemberInfo IdMember { get { return idMember; } }
		private MemberInfo idMember;
		public IEnumerable<MemberInfo> OrdinaryMembers { get { return ordinaryMembers; } }
		private IList<MemberInfo> ordinaryMembers;
		public string ORM { get { return DapperDatabaseContext.ConstORM; } }

		/// <summary>
		/// Initialize
		/// </summary>
		public DapperEntityMappingBuilder() {
			tableName = typeof(T).Name;
			idMember = null;
			ordinaryMembers = new List<MemberInfo>();
			// Configure with registered providers
			var providers = Application.Ioc.ResolveMany<IEntityMappingProvider<T>>();
			foreach (var provider in providers) {
				provider.Configure(this);
			}
			// Set table name with registered handlers
			var handlers = Application.Ioc.ResolveMany<IDatabaseInitializeHandler>();
			handlers.ForEach(h => h.ConvertTableName(ref tableName));
		}

		/// <summary>
		/// Specify the primary key for this entity
		/// </summary>
		public void Id<TPrimaryKey>(
			Expression<Func<T, TPrimaryKey>> memberExpression,
			EntityMappingOptions options) {
			idMember = ((MemberExpression)memberExpression.Body).Member;
		}

		/// <summary>
		/// Create a member mapping
		/// </summary>
		public void Map<TMember>(
			Expression<Func<T, TMember>> memberExpression,
			EntityMappingOptions options) {
			ordinaryMembers.Add(((MemberExpression)memberExpression.Body).Member);
		}

		/// <summary>
		/// Create a reference to another entity, this is a many-to-one relationship.
		/// </summary>
		public void References<TOther>(
			Expression<Func<T, TOther>> memberExpression,
			EntityMappingOptions options)
			where TOther : class {
			throw new NotSupportedException("References is not supported with dapper");
		}

		/// <summary>
		/// Maps a collection of entities as a one-to-many relationship.
		/// </summary>
		public void HasMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options)
			where TChild : class {
			throw new NotSupportedException("HasMany is not supported with dapper");
		}

		/// <summary>
		/// Maps a collection of entities as a many-to-many relationship.
		/// </summary>
		public void HasManyToMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options = null)
			where TChild : class {
			throw new NotSupportedException("HasManyToMany is not supported with dapper");
		}
	}
}
