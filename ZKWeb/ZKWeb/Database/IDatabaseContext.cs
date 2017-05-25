using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace ZKWeb.Database {
	/// <summary>
	/// Interface for database context<br/>
	/// 数据库上下文的接口<br/>
	/// </summary>
	/// <example>
	/// Example for insert enity<br/>
	/// 添加实体的示例<br/>
	/// <code lannguage="cs">
	/// var databaseManager = Application.Ioc.Resolve&lt;DatabaseManager&gt;();
	/// using (var context = databaseManager.CreateContext()) {
	///		var data = new ExampleTable() {
	///			Name = "test",
	///			CreateTime = DateTime.UtcNow,
	///			Deleted = false
	///		};
	///		context.Save(ref data);
	/// }
	/// </code>
	/// 
	/// Example for update entity<br/>
	/// 修改实体的示例<br/>
	/// <code language="cs">
	/// var databaseManager = Application.Ioc.Resolve&lt;DatabaseManager&gt;();
	/// using (var context = databaseManager.CreateContext()) {
	///		var data = context.Get&lt;ExampleTable&gt;(t => t.name == "test");
	///		context.Save(ref data, d => d.Name = "updated");
	/// }
	/// </code>
	/// 
	/// Example for query entities<br/>
	/// 查询实体的示例<br/>
	/// <code language="cs">
	/// var databaseManager = Application.Ioc.Resolve&lt;DatabaseManager&gt;();
	/// using (var context = databaseManager.CreateContext()) {
	///		var notUpdated = context.Query&lt;ExampleTable&gt;().Where(t => t.Name != "updated").ToList();
	/// }
	/// </code>
	/// 
	/// Example for delete entities<br/>
	/// 删除数据的示例<br/>
	/// <code language="cs">
	/// var databaseManager = Application.Ioc.Resolve&lt;DatabaseManager&gt;();
	/// using (var context = databaseManager.CreateContext()) {
	///		long deleted = context.BatchDelete&lt;ExampleTable&gt;(d => d.Name == "updated");
	/// }
	/// </code> 
	/// </example>
	/// <seealso cref="IDatabaseContextFactory"/>
	/// <seealso cref="DatabaseManager"/>
	public interface IDatabaseContext : IDisposable {
		/// <summary>
		/// ORM name<br/>
		/// ORM名称<br/>
		/// </summary>
		/// <example>NHibernate, EFCore</example>
		string ORM { get; }
		/// <summary>
		/// Database type<br/>
		/// 数据库类型<br/>
		/// </summary>
		/// <example>MSSQL, SQLite</example>
		string Database { get; }
		/// <summary>
		/// Underlying database connection<br/>
		/// It may return any types of object or just return null<br/>
		/// 下层的数据库连接<br/>
		/// 它可能返回任何类型的对象或返回null<br/>
		/// </summary>
		object DbConnection { get; }

		/// <summary>
		/// Begin a transaction<br/>
		/// It should support nested calls<br/>
		/// Only the outermost call will actually create the transaction, and the inner call will only increase the counter<br/>
		/// Attention: Not all ORMs support this feature<br/>
		/// 开始一个事务<br/>
		/// 它应该支持嵌套调用<br/>
		/// 只有最外层的调用会实际的创建事务, 里层的调用只会增加计数器<br/>
		/// 注意: 不是所有ORM都支持这个功能<br/>
		/// </summary>
		/// <param name="isolationLevel">The isolation level</param>
		void BeginTransaction(IsolationLevel? isolationLevel = null);

		/// <summary>
		/// Finish the transaction<br/>
		/// It should support nested calls<br/>
		/// Only the outermost call will commit the transaction, and the inner call will only reduce the counter<br/>
		/// Attention: Not all ORMs support this feature<br/>
		/// 结束一个事务<br/>
		/// 它应该支持嵌套调用<br/>
		/// 只有最外层的调用会提交事务, 里层的调用只会减少计数器<br/>
		/// 注意: 不是所有ORM都支持这个功能<br/>
		/// </summary>
		void FinishTransaction();

		/// <summary>
		/// Get the query object for specific entity type<br/>
		/// 获取指定实体类型的查询对象<br/>
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <returns></returns>
		IQueryable<T> Query<T>()
			where T : class, IEntity;

		/// <summary>
		/// Get single entity that matched the given predicate<br/>
		/// It should return null if no matched entity found<br/>
		/// 获取符合传入条件的单个实体<br/>
		/// 如果无符合条件的实体应该返回null<br/>
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <param name="predicate">The predicate</param>
		/// <returns></returns>
		T Get<T>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity;

		/// <summary>
		/// Get how many entities that matched the given predicate<br/>
		/// 获取符合传入条件的实体数量<br/>
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <param name="predicate">The predicate</param>
		/// <returns></returns>
		long Count<T>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity;

		/// <summary>
		/// Save entity to database<br/>
		/// It should notify registered handlers of IEntityOperationHandler<br/>
		/// Update action should be executed between BeforeSave and AfterSave method<br/>
		/// 保存实体到数据库<br/>
		/// 它应该通知注册的IEntityOperationHandler处理器<br/>
		/// 更新函数应该在BeforeSave和AfterSave函数之间执行<br/>
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <param name="entity">Entity object, may get replaced if not tracked by ORM</param>
		/// <param name="update">Update action</param>
		void Save<T>(ref T entity, Action<T> update = null)
			where T : class, IEntity;

		/// <summary>
		/// Delete entity from database<br/>
		/// It should notify registered handlers of IEntityOperationHandler<br/>
		/// 删除数据库中的实体<br/>
		/// 它应该通知注册的IEntityOperationHandler处理器<br/>
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <param name="entity">Entity object</param>
		void Delete<T>(T entity)
			where T : class, IEntity;

		/// <summary>
		/// Batch save entities<br/>
		/// It should notify registered handlers of IEntityOperationHandler<br/>
		/// Update action should be executed between BeforeSave and AfterSave method<br/>
		/// 批量保存实体<br/>
		/// 它应该通知注册的IEntityOperationHandler处理器<br/>
		/// 更新函数应该在BeforeSave和AfterSave函数之间执行<br/>
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <param name="entities">Entity objects</param>
		/// <param name="update">Update action</param>
		void BatchSave<T>(ref IEnumerable<T> entities, Action<T> update = null)
			where T : class, IEntity;

		/// <summary>
		/// Batch update entities<br/>
		/// Return how many entities been updated<br/>
		/// It should notify registered handlers of IEntityOperationHandler<br/>
		/// 批量更新实体<br/>
		/// 返回更新的实体数量<br/>
		/// 它应该通知注册的IEntityOperationHandler处理器<br/>
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <param name="predicate">The predicate</param>
		/// <param name="update">Update action</param>
		/// <returns></returns>
		long BatchUpdate<T>(Expression<Func<T, bool>> predicate, Action<T> update)
			where T : class, IEntity;

		/// <summary>
		/// Batch delete entities<br/>
		/// Return how many entities been deleted<br/>
		/// It should notify registered handlers of IEntityOperationHandler<br/>
		/// 批量删除实体<br/>
		/// 返回删除的实体数量<br/>
		/// 它应该通知注册的IEntityOperationHandler处理器<br/>
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <param name="predicate">The predicate</param>
		/// <param name="beforeDelete">Action before delete</param>
		/// <returns></returns>
		long BatchDelete<T>(
			Expression<Func<T, bool>> predicate, Action<T> beforeDelete = null)
			where T : class, IEntity;

		/// <summary>
		/// Batch save entities in faster way<br/>
		/// It wouldn't call registered handlers of IEntityOperationHandler<br/>
		/// 快速批量保存实体<br/>
		/// 它不会通知注册的IEntityOperationHandler处理器<br/>
		/// </summary>
		/// <typeparam name="T">Entity type</typeparam>
		/// <typeparam name="TPrimaryKey">Primary key type</typeparam>
		/// <param name="entities">Entity objects</param>
		void FastBatchSave<T, TPrimaryKey>(IEnumerable<T> entities)
			where T : class, IEntity<TPrimaryKey>;

		/// <summary>
		/// Batch delete entities in faster way<br/>
		/// It wouldn't call registered handlers of IEntityOperationHandler<br/>
		/// 快速批量删除实体<br/>
		/// 它不会通知注册的IEntityOperationHandler处理器<br/>
		/// </summary>
		/// <typeparam name="T">Entity type</typeparam>
		/// <typeparam name="TPrimaryKey">Primary key type</typeparam>>
		/// <param name="predicate">The predicate</param>
		/// <returns></returns>
		long FastBatchDelete<T, TPrimaryKey>(Expression<Func<T, bool>> predicate)
			where T : class, IEntity<TPrimaryKey>, new();

		/// <summary>
		/// Perform a raw update to database<br/>
		/// No operation handlers will be called<br/>
		/// parameter "query" and "parameters" are ORM and Database specified<br/>
		/// 执行一个原生的更新操作<br/>
		/// 不会调用操作处理器<br/>
		/// 参数"query"和"parameters"的类型根据ORM和数据库而定<br/>
		/// </summary>
		/// <param name="query">Query object (eg: string)</param>
		/// <param name="parameters">Query parameters (eg: IDictionary[String, object])</param>
		/// <returns></returns>
		long RawUpdate(object query, object parameters);

		/// <summary>
		/// Perform a raw query to database<br/>
		/// No operation handlers will be called<br/>
		/// parameter "query" and "parameters" are ORM and Database specified<br/>
		/// 执行一个原生的查询操作<br/>
		/// 不会调用操作处理器<br/>
		/// 参数"query"和"parameters"的类型根据ORM和数据库而定<br/>
		/// </summary>
		/// <typeparam name="T">Entity Type</typeparam>
		/// <param name="query">Query object (eg: string)</param>
		/// <param name="parameters">Query parameters (eg: SqlParameter[])</param>
		/// <returns></returns>
		IEnumerable<T> RawQuery<T>(object query, object parameters)
			where T : class;
	}
}
