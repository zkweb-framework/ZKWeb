namespace ZKWeb.Database {
	/// <summary>
	/// Entity mapping options
	/// </summary>
	public class EntityMappingOptions {
		/// <summary>
		/// Column name
		/// Will use this name instead of the default if not empty
		/// </summary>
		public string Column { get; set; }
		/// <summary>
		/// Field length
		/// Will setting the field length if not null
		/// </summary>
		public long? Length { get; set; }
		/// <summary>
		/// Is unique field
		/// Will create a unique index if not empty
		/// </summary>
		public bool? Unique { get; set; }
		/// <summary>
		/// Is nullable field
		/// Will specific field is nullable or not if not null
		/// </summary>
		public bool? Nullable { get; set; }
		/// <summary>
		/// Index name
		/// Will create an index with this name if not empty
		/// </summary>
		public string Index { get; set; }
		/// <summary>
		/// Custom sql type
		/// Will use this type instead the default if not empty
		/// </summary>
		public string CustomSqlType { get; set; }
		/// <summary>
		/// Enable cascade delete
		/// Will specific cascade delete or do nothing if not null
		/// </summary>
		public bool? CascadeDelete { get; set; }
		/// <summary>
		/// Member will serialize (eg: as json) before store into database,
		/// and deserialize after retrieve from database if it is true
		/// </summary>
		public bool? WithSerialization { get; set; }
	}
}
