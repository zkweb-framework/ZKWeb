using System.Collections.Generic;
using ZKWeb.Database;

namespace ZKWeb.ORM.InMemory {
	/// <summary>
	/// InMemory database context factory<br/>
	/// This factory hold a memory database for all context it created<br/>
	/// 内存数据库的上下文生成器<br/>
	/// 这个生成器应该包含一个内存数据库供所有生成的上下文使用<br/>
	/// </summary>
	public class InMemoryDatabaseContextFactory : IDatabaseContextFactory {
		/// <summary>
		/// The store object<br/>
		/// 储存对象<br/>
		/// </summary>
		protected InMemoryDatabaseStore Store { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public InMemoryDatabaseContextFactory(string database, string connectionString) :
			this(database, connectionString,
				Application.Ioc.ResolveMany<IDatabaseInitializeHandler>(),
				Application.Ioc.ResolveMany<IEntityMappingProvider>()) { }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public InMemoryDatabaseContextFactory(
			string database, string connectionString,
			IEnumerable<IDatabaseInitializeHandler> handlers,
			IEnumerable<IEntityMappingProvider> providers) {
			Store = new InMemoryDatabaseStore(handlers, providers);
		}

		/// <summary>
		/// Create a database context<br/>
		/// 创建数据库上下文<br/>
		/// </summary>
		/// <returns></returns>
		public IDatabaseContext CreateContext() {
			return new InMemoryDatabaseContext(Store);
		}
	}
}
