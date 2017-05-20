namespace ZKWeb.Database {
	/// <summary>
	/// Interface for handling entity modification and deletion<br/>
	/// 用于处理实体修改和删除的接口<br/>
	/// </summary>
	/// <seealso cref="IDatabaseContext"/>
	/// <seealso cref="IEntity{TPrimaryKey}"/>
	/// <seealso cref="DatabaseManager"/>
	/// <example>
	/// <code language="cs">
	/// [ExportMany]
	/// public class ExampleEntityOperationHandler : IEntityOperationHandler&lt;ExampleTable&gt; {
	///		private long IdBeforeSave { get; set; }
	///	 	private string NameBeforeSave { get; set; }
	///
	///		public void BeforeSave(IDatabaseContext context, ExampleTable data) {
	///			IdBeforeSave = data.Id;
	///			NameBeforeSave = data.Name;
	///		}
	///
	///		public void AfterSave(IDatabaseContext context, ExampleTable data) {
	///			var logManager = Application.Ioc.Resolve&lt;LogManager&gt;();
	///			if (IdBeforeSave &lt;= 0) {
	///				logManager.LogDebug(string.Format("example data inserted, id is {0}", data.Id));
	///			} else if (NameBeforeSave != data.Name) {
	///				logManager.LogDebug(string.Format("example data name changed, id is {0}", data.Id));
	///			}
	///		}
	///
	///		public void BeforeDelete(IDatabaseContext context, ExampleTable data) {
	///		}
	///
	///		public void AfterDelete(IDatabaseContext context, ExampleTable data) {
	///			var logManager = Application.Ioc.Resolve&lt;LogManager&gt;();
	///			logManager.LogDebug(string.Format("example data deleted, id is {0}", data.Id));
	///		}
	/// }
	/// </code>
	/// </example>
	public interface IEntityOperationHandler<T>
		where T : class, IEntity {
		/// <summary>
		/// Execute before save<br/>
		/// 保存前(修改前)的处理<br/>
		/// </summary>
		/// <param name="context">Database context</param>
		/// <param name="entity">Entity object</param>
		void BeforeSave(IDatabaseContext context, T entity);

		/// <summary>
		/// Execute after saved<br/>
		/// 保存后(修改后)的处理<br/>
		/// </summary>
		/// <param name="context">Database context</param>
		/// <param name="entity">Entity object</param>
		void AfterSave(IDatabaseContext context, T entity);

		/// <summary>
		/// Execute before delete<br/>
		/// 删除前的处理<br/>
		/// </summary>
		/// <param name="context">Database context</param>
		/// <param name="entity">Entity object</param>
		void BeforeDelete(IDatabaseContext context, T entity);

		/// <summary>
		/// Execute after deleted<br/>
		/// 删除后的处理<br/>
		/// </summary>
		/// <param name="context">Database context</param>
		/// <param name="entity">Entity object</param>
		void AfterDelete(IDatabaseContext context, T entity);
	}
}
