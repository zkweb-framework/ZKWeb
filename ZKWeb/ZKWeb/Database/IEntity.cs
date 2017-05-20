namespace ZKWeb.Database {
	/// <summary>
	/// Non-generic interface of entity<br/>
	/// 实体的非泛型接口<br/>
	/// </summary>
	/// <seealso cref="IEntityMappingProvider"/>
	/// <seealso cref="IDatabaseContext"/>
	/// <seealso cref="IEntity{TPrimaryKey}"/>
	public interface IEntity { }

	/// <summary>
	/// Generic interface of entity<br/>
	/// 实体的泛型接口<br/>
	/// </summary>
	/// <typeparam name="TPrimaryKey">Primary key type</typeparam>
	/// <seealso cref="IEntityMappingProvider"/>
	/// <seealso cref="IDatabaseContext"/>
	/// <seealso cref="IEntity"/>
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
	public interface IEntity<TPrimaryKey> : IEntity {
		/// <summary>
		/// Primary key<br/>
		/// 主键<br/>
		/// </summary>
		TPrimaryKey Id { get; set; }
	}
}
