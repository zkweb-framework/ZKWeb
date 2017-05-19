namespace ZKWeb.Database {
	/// <summary>
	/// The mapping options of the entity field<br/>
	/// 实体字段的映射选项<br/>
	/// </summary>
	public class EntityMappingOptions {
		/// <summary>
		/// Custom column name<br/>
		/// 自定义列名<br/>
		/// </summary>
		public string Column { get; set; }
		/// <summary>
		/// Custom field length<br/>
		/// 自定义字段长度<br/>
		/// </summary>
		public long? Length { get; set; }
		/// <summary>
		/// Create unique index<br/>
		/// 是否创建唯一键<br/>
		/// </summary>
		public bool? Unique { get; set; }
		/// <summary>
		/// Allow this field to be null<br/>
		/// 是否允许这个字段等于null<br/>
		/// </summary>
		public bool? Nullable { get; set; }
		/// <summary>
		/// Create index with this name<br/>
		/// 是否使用此名称创建索引<br/>
		/// </summary>
		public string Index { get; set; }
		/// <summary>
		/// Custom sql type<br/>
		/// 自定义sql类型<br/>
		/// </summary>
		public string CustomSqlType { get; set; }
		/// <summary>
		/// Enable cascade delete<br/>
		/// 是否开启级联删除<br/>
		/// </summary>
		public bool? CascadeDelete { get; set; }
		/// <summary>
		/// Member will serialize (eg: as json) before store into database,<br/>
		/// and deserialize after retrieve from database if it is true<br/>
		/// 如果开启了此选项是, 则会在保存成员时序列化(例如json), 并在获取成员时反序列化<br/>
		/// </summary>
		public bool? WithSerialization { get; set; }
		/// <summary>
		/// Custom navigation property on the other side for References, HasMany, HasManyToMany<br/>
		/// If this option is empty, it will be detected automatically<br/>
		/// 自定义的导航属性, 用于指定References, HasMany, HasManyToMany的另一边实体中的属性名称<br/>
		/// 如果这个选项空白, 则会自动检测该属性<br/>
		/// </summary>
		public string Navigation { get; set; }
	}
}
