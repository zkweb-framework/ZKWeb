using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ZKWeb.Database;

namespace ZKWeb.ORM.InMemory {
	/// <summary>
	/// Defines a mapping for an entity<br/>
	/// 定义实体的映射<br/>
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
		public string ORM { get { return InMemoryDatabaseContext.ConstORM; } }
		public object NativeBuilder { get { return this; } set { } }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public InMemoryEntityMappingBuilder() {
			OrdinaryMembers = new List<MemberInfo>();
			ManyToOneMembers = new List<MemberInfo>();
			OneToManyMembers = new List<MemberInfo>();
			ManyToManyMembers = new List<MemberInfo>();
			// Configure with registered providers
			var providers = Application.Ioc.ResolveMany<IEntityMappingProvider<T>>();
			foreach (var provider in providers) {
				provider.Configure(this);
			}
		}

		/// <summary>
		/// Specify the primary key for this entity<br/>
		/// 指定实体的主键<br/>
		/// </summary>
		public void Id<TPrimaryKey>(
			Expression<Func<T, TPrimaryKey>> memberExpression,
			EntityMappingOptions options) {
			IdMember = ((MemberExpression)memberExpression.Body).Member;
		}

		/// <summary>
		/// Create a member mapping<br/>
		/// 创建成员映射<br/>
		/// </summary>
		public void Map<TMember>(
			Expression<Func<T, TMember>> memberExpression,
			EntityMappingOptions options) {
			OrdinaryMembers.Add(((MemberExpression)memberExpression.Body).Member);
		}

		/// <summary>
		/// Create a reference to another entity, this is a many-to-one relationship.<br/>
		/// 创建到其他实体的映射, 这是多对一的关系<br/>
		/// </summary>
		public void References<TOther>(
			Expression<Func<T, TOther>> memberExpression,
			EntityMappingOptions options)
			where TOther : class {
			ManyToOneMembers.Add(((MemberExpression)memberExpression.Body).Member);
		}

		/// <summary>
		/// Maps a collection of entities as a one-to-many relationship.<br/>
		/// 创建到实体集合的映射, 这是一对多的关系<br/>
		/// </summary>
		public void HasMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options)
			where TChild : class {
			OneToManyMembers.Add(((MemberExpression)memberExpression.Body).Member);
		}

		/// <summary>
		/// Maps a collection of entities as a many-to-many relationship.<br/>
		/// 创建到实体集合的映射, 这是多对多的关系<br/>
		/// </summary>
		public void HasManyToMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression,
			EntityMappingOptions options = null)
			where TChild : class {
			ManyToManyMembers.Add(((MemberExpression)memberExpression.Body).Member);
		}
	}
}
