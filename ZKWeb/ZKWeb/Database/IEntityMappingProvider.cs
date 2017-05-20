namespace ZKWeb.Database {
	/// <summary>
	/// Empty base interface<br/>
	/// 空的基础接口<br/>
	/// </summary>
	/// <seealso cref="IEntityMappingProvider{T}"/>
	public interface IEntityMappingProvider { }

	/// <summary>
	/// Interface used to alter the entity mapping builder<br/>
	/// You should define this when you define an new entity<br/>
	/// 用于构建实体映射的接口<br/>
	/// 你应该在定义一个新的实体时同时定义这个提供器<br/>
	/// </summary>
	/// <typeparam name="T">Entity Type</typeparam>
	/// <seealso cref="IEntity{TPrimaryKey}"/>
	/// <seealso cref="IEntityMappingBuilder{T}"/>
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
	public interface IEntityMappingProvider<T> : IEntityMappingProvider
		where T : class, IEntity {
		/// <summary>
		/// Configure entity mapping<br/>
		/// 配置实体映射<br/>
		/// </summary>
		/// <param name="builder">Entity mapping builder</param>
		void Configure(IEntityMappingBuilder<T> builder);
	}
}
