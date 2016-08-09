namespace ZKWeb.Database {
	/// <summary>
	/// Interface for handling entity modification and deletion 
	/// </summary>
	public interface IEntityOperationHandler<T>
		where T : class, IEntity {
		/// <summary>
		/// Execute before save
		/// </summary>
		/// <param name="context">Database context</param>
		/// <param name="entity">Entity object</param>
		void BeforeSave(IDatabaseContext context, T entity);

		/// <summary>
		/// Execute after saved
		/// </summary>
		/// <param name="context">Database context</param>
		/// <param name="entity">Entity object</param>
		void AfterSave(IDatabaseContext context, T entity);

		/// <summary>
		/// Execute before delete
		/// </summary>
		/// <param name="context">Database context</param>
		/// <param name="entity">Entity object</param>
		void BeforeDelete(IDatabaseContext context, T entity);

		/// <summary>
		/// Execute after deleted
		/// </summary>
		/// <param name="context">Database context</param>
		/// <param name="entity">Entity object</param>
		void AfterDelete(IDatabaseContext context, T entity);
	}
}
