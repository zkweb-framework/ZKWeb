using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ZKWeb.Database;

namespace ZKWeb.ORM.InMemory {
	/// <summary>
	/// Defines a mapping for an entity
	/// </summary>
	/// <typeparam name="T">Entity type</typeparam>
	public class InMemoryEntityMappingBuilder<T> : IEntityMappingBuilder<T>
		where T : class, IEntity {
		/// <summary>
		/// Specify the primary key for this entity
		/// </summary>
		public void Id<TPrimaryKey>(
			Expression<Func<T, TPrimaryKey>> memberExpression,
			EntityMappingOptions options = null) {
			
		}

		/// <summary>
		/// Create a member mapping
		/// </summary>
		public void Map<TMember>(
			Expression<Func<T, TMember>> memberExpression,
			EntityMappingOptions options = null) {
			
		}

		/// <summary>
		/// Create a reference to another entity, this is a many-to-one relationship.
		/// </summary>
		public void References<TOther>(
			Expression<Func<T, TOther>> memberExpression,
			EntityMappingOptions options = null) {

		}

		/// <summary>
		/// Maps a collection of entities as a one-to-many relationship.
		/// </summary>
		public void HasMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options = null) {

		}

		/// <summary>
		/// Maps a collection of entities as a many-to-many relationship.
		/// </summary>
		public void HasManyToMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options = null) {

		}
	}
}
