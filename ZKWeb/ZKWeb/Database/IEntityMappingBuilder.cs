using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ZKWeb.Database {
	/// <summary>
	/// Interface for defines a mapping for an entity
	/// </summary>
	/// <typeparam name="T">Entity Type</typeparam>
	public interface IEntityMappingBuilder<T>
		where T : class, IEntity {
		/// <summary>
		/// ORM name
		/// </summary>
		string ORM { get; }

		/// <summary>
		/// Specify the primary key for this entity
		/// </summary>
		/// <typeparam name="TPrimaryKey">Primary key type</typeparam>
		/// <param name="memberExpression">Expression that access the primary key</param>
		/// <param name="options">Mapping options</param>
		void Id<TPrimaryKey>(Expression<Func<T, TPrimaryKey>> memberExpression, EntityMappingOptions options = null);

		/// <summary>
		/// Create a member mapping
		/// </summary>
		/// <typeparam name="TMember">Member type</typeparam>
		/// <param name="memberExpression">Expression that access the member</param>
		/// <param name="options">Mapping options</param>
		void Map<TMember>(Expression<Func<T, TMember>> memberExpression, EntityMappingOptions options = null);

		/// <summary>
		/// Create a reference to another entity, this is a many-to-one relationship.
		/// </summary>
		/// <typeparam name="TOther">Other entity type</typeparam>
		/// <param name="memberExpression">Expression that access the reference member</param>
		/// <param name="options">Mapping options</param>
		void References<TOther>(
			Expression<Func<T, TOther>> memberExpression, EntityMappingOptions options = null)
			where TOther : class;

		/// <summary>
		/// Maps a collection of entities as a one-to-many relationship.
		/// </summary>
		/// <typeparam name="TChild">Child entity type</typeparam>
		/// <param name="memberExpression">Expression that access the collection member</param>
		/// <param name="options">Mapping options</param>
		void HasMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression, EntityMappingOptions options = null)
			where TChild : class;

		/// <summary>
		/// Maps a collection of entities as a many-to-many relationship.
		/// </summary>
		/// <typeparam name="TChild">Child entity type</typeparam>
		/// <param name="memberExpression">Expression that access the collection member</param>
		/// <param name="options">Mapping options</param>
		void HasManyToMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression, EntityMappingOptions options = null)
			where TChild : class;
	}
}
