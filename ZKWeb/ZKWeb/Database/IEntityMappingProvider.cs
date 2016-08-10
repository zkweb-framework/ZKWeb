namespace ZKWeb.Database {
	/// <summary>
	/// Empty base interface
	/// </summary>
	public interface IEntityMappingProvider { }

	/// <summary>
	/// Interface used to alter the entity mapping builder
	/// You should define this when you define an new entity
	/// </summary>
	/// <typeparam name="T">Entity Type</typeparam>
	public interface IEntityMappingProvider<T> : IEntityMappingProvider
		where T : class, IEntity {
		/// <summary>
		/// Configure entity mapping
		/// </summary>
		/// <param name="builder">Entity mapping builder</param>
		void Configure(IEntityMappingBuilder<T> builder);
	}
}
