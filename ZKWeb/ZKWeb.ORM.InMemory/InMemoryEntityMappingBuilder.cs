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
	internal class InMemoryEntityMappingBuilder<T> :
		IEntityMappingBuilder<T>, IInMemoryEntityMapping
		where T : class, IEntity {
		public Type EntityType { get { return typeof(T); } }
		public MemberInfo IdMember { get; set; }
		public IList<MemberInfo> OrdinaryMembers { get; set; }
		public IList<MemberInfo> ManyToOneMembers { get; set; }
		public IList<MemberInfo> OneToManyMembers { get; set; }
		public IList<MemberInfo> ManyToManyMembers { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public InMemoryEntityMappingBuilder() {
			OrdinaryMembers = new List<MemberInfo>();
			ManyToOneMembers = new List<MemberInfo>();
			OneToManyMembers = new List<MemberInfo>();
			ManyToManyMembers = new List<MemberInfo>();
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
			EntityMappingOptions options) {
			ManyToOneMembers.Add(((MemberExpression)memberExpression.Body).Member);
		}

		/// <summary>
		/// Maps a collection of entities as a one-to-many relationship.
		/// </summary>
		public void HasMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options) {
			OneToManyMembers.Add(((MemberExpression)memberExpression.Body).Member);
		}

		/// <summary>
		/// Maps a collection of entities as a many-to-many relationship.
		/// </summary>
		public void HasManyToMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options = null) {
			ManyToManyMembers.Add(((MemberExpression)memberExpression.Body).Member);
		}
	}
}
