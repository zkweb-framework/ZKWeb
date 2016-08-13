using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ZKWeb.Database;

namespace ZKWeb.ORM.Dapper {
	/// <summary>
	/// Defines a mapping for an entity
	/// </summary>
	/// <typeparam name="T">Entity type</typeparam>
	internal class DapperEntityMappingBuilder<T> :
		IEntityMappingBuilder<T>, IDapperEntityMapping
		where T : class, IEntity {
		public Type EntityType { get { return typeof(T); } }
		public MemberInfo IdMember { get; set; }
		public IList<MemberInfo> OrdinaryMembers { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public DapperEntityMappingBuilder() {
			OrdinaryMembers = new List<MemberInfo>();
		}

		/// <summary>
		/// Specify the primary key for this entity
		/// </summary>
		public void Id<TPrimaryKey>(
			Expression<Func<T, TPrimaryKey>> memberExpression,
			EntityMappingOptions options) {
			IdMember = ((MemberExpression)memberExpression.Body).Member;
		}

		/// <summary>
		/// Create a member mapping
		/// </summary>
		public void Map<TMember>(
			Expression<Func<T, TMember>> memberExpression,
			EntityMappingOptions options) {
			OrdinaryMembers.Add(((MemberExpression)memberExpression.Body).Member);
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
