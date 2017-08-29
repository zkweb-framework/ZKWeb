using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ZKWeb.Database;

namespace ZKWeb.ORM.InMemory {
	/// <summary>
	/// Defines a mapping for an entity<br/>
	/// 定义实体的映射<br/>
	/// </summary>
	/// <typeparam name="T">Entity type</typeparam>
	public class InMemoryEntityMappingBuilder<T> :
		IEntityMappingBuilder<T>,
		IInMemoryEntityMapping
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
		public MemberInfo IdMember { get; set; }
		/// <summary>
		/// Ordinary members<br/>
		/// 普通成员列表<br/>
		/// </summary>
		public IList<MemberInfo> OrdinaryMembers { get; set; }
		/// <summary>
		/// Many-to-one members<br/>
		/// 多对一的成员列表<br/>
		/// </summary>
		public IList<MemberInfo> ManyToOneMembers { get; set; }
		/// <summary>
		/// One-to-many embers<br/>
		/// 一对多的成员列表<br/>
		/// </summary>
		public IList<MemberInfo> OneToManyMembers { get; set; }
		/// <summary>
		/// Many-to-many members<br/>
		/// 多对多的成员列表<br/>
		/// </summary>
		public IList<MemberInfo> ManyToManyMembers { get; set; }
		/// <summary>
		/// ORM name<br/>
		/// ORM名称<br/>
		/// </summary>
		public string ORM { get { return InMemoryDatabaseContext.ConstORM; } }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public InMemoryEntityMappingBuilder(
			IEnumerable<IDatabaseInitializeHandler> handlers,
			IEnumerable<IEntityMappingProvider> providers) {
			OrdinaryMembers = new List<MemberInfo>();
			ManyToOneMembers = new List<MemberInfo>();
			OneToManyMembers = new List<MemberInfo>();
			ManyToManyMembers = new List<MemberInfo>();
			// Configure with registered providers
			foreach (IEntityMappingProvider<T> provider in providers) {
				provider.Configure(this);
			}
		}

		/// <summary>
		/// Specify the custom table name<br/>
		/// 指定自定义表名<br/>
		/// </summary>
		/// <param name="tableName">The table name</param>
		public void TableName(string tableName) {
			// Do nothing
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
