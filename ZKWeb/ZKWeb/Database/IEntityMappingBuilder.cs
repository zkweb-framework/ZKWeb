using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ZKWeb.Database {
	/// <summary>
	/// Interface used to define an entity-to-database mapping<br/>
	/// 用于定义实体到数据库映射的接口<br/>
	/// </summary>
	/// <typeparam name="T">Entity Type</typeparam>
	/// <example>
	/// <code language="cs">
	/// [ExportMany]
	/// public class ExampleTable : IEntity&lt;long&gt;, IEntityMappingProvider&lt;ExampleTable&gt; {
	///		public virtual long Id { get; set; }
	///		public virtual string Name { get; set; }
	///		public virtual DateTime CreateTime { get; set; }
	///
	///		public virtual void Configure(IEntityMappingBuilder&lt;ExampleTable&gt; builder) {
	///			builder.Id(e => e.Id);
	///			builder.Map(e => e.Name);
	///			builder.Map(e => e.CreateTime);
	///		}
	/// }
	/// </code>
	/// </example>
	public interface IEntityMappingBuilder<T>
		where T : class, IEntity {
		/// <summary>
		/// ORM name<br/>
		/// ORM名称<br/>
		/// </summary>
		string ORM { get; }

		/// <summary>
		/// Specify the custom table name<br/>
		/// 指定自定义表名<br/>
		/// </summary>
		/// <param name="tableName">The table name</param>
		void TableName(string tableName);

		/// <summary>
		/// Specify the primary key for this entity<br/>
		/// 指定实体的主键<br/>
		/// </summary>
		/// <typeparam name="TPrimaryKey">Primary key type</typeparam>
		/// <param name="memberExpression">Expression that access the primary key</param>
		/// <param name="options">Mapping options</param>
		void Id<TPrimaryKey>(Expression<Func<T, TPrimaryKey>> memberExpression, EntityMappingOptions options = null);

		/// <summary>
		/// Create a member mapping<br/>
		/// 创建成员映射<br/>
		/// </summary>
		/// <typeparam name="TMember">Member type</typeparam>
		/// <param name="memberExpression">Expression that access the member</param>
		/// <param name="options">Mapping options</param>
		void Map<TMember>(Expression<Func<T, TMember>> memberExpression, EntityMappingOptions options = null);

		/// <summary>
		/// Create a reference to another entity, this is a many-to-one relationship.<br/>
		/// 创建到其他实体的映射, 这是多对一的关系<br/>
		/// </summary>
		/// <typeparam name="TOther">Other entity type</typeparam>
		/// <param name="memberExpression">Expression that access the reference member</param>
		/// <param name="options">Mapping options</param>
		void References<TOther>(
			Expression<Func<T, TOther>> memberExpression, EntityMappingOptions options = null)
			where TOther : class;

		/// <summary>
		/// Maps a collection of entities as a one-to-many relationship.<br/>
		/// 创建到实体集合的映射, 这是一对多的关系<br/>
		/// </summary>
		/// <typeparam name="TChild">Child entity type</typeparam>
		/// <param name="memberExpression">Expression that access the collection member</param>
		/// <param name="options">Mapping options</param>
		void HasMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression, EntityMappingOptions options = null)
			where TChild : class;

		/// <summary>
		/// Maps a collection of entities as a many-to-many relationship.<br/>
		/// 创建到实体集合的映射, 这是多对多的关系<br/>
		/// </summary>
		/// <typeparam name="TChild">Child entity type</typeparam>
		/// <param name="memberExpression">Expression that access the collection member</param>
		/// <param name="options">Mapping options</param>
		void HasManyToMany<TChild>(
			Expression<Func<T, IEnumerable<TChild>>> memberExpression, EntityMappingOptions options = null)
			where TChild : class;
	}
}
