using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using ZKWeb.Database;

namespace ZKWeb.ORM.EFCore {
	/// <summary>
	/// Entity Framework Core database context factory
	/// </summary>
	internal class EFCoreDatabaseContextFactory : IDatabaseContextFactory {
		/// <summary>
		/// Database type
		/// </summary>
		private string Database { get; set; }
		/// <summary>
		/// Connection string
		/// </summary>
		private string ConnectionString { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="database">Database type</param>
		/// <param name="connectionString">Connection string</param>
		public EFCoreDatabaseContextFactory(string database, string connectionString) {
			Database = database;
			ConnectionString = connectionString;
			// Initialize database scheme
			using (var context = (DbContext)CreateContext()) {
				// TODO: create a empty database first
				// think about how to store and load IModel (use GenerateSnapshot?)
				var serviceProvider = ((IInfrastructure<IServiceProvider>)context).Instance;
				var databaseCreator = serviceProvider.GetService<IDatabaseCreator>();
				if (databaseCreator is IRelationalDatabaseCreator) {
					// It's a relational database, create and apply the migration
					// Also see: https://github.com/aspnet/EntityFramework/blob/master/src/Microsoft.EntityFrameworkCore.Relational/Storage/RelationalDatabaseCreator.cs
					var modelDiffer = serviceProvider.GetService<IMigrationsModelDiffer>();
					var sqlGenerator = serviceProvider.GetService<IMigrationsSqlGenerator>();
					var commandExecutor = serviceProvider.GetService<IMigrationCommandExecutor>();
					var operations = modelDiffer.GetDifferences(null, context.Model);
					var commands = sqlGenerator.Generate(operations, context.Model);
					var connection = serviceProvider.GetService<IRelationalConnection>();
					try { context.Database.ExecuteSqlCommand("select 1"); } catch { }
					commandExecutor.ExecuteNonQuery(commands, connection);
				} else {
					// It maybe a in-memory database or no-sql database, do nothing
				}
			}
		}

		/// <summary>
		/// Create database context
		/// </summary>
		/// <returns></returns>
		public IDatabaseContext CreateContext() {
			return new EFCoreDatabaseContext(Database, ConnectionString);
		}
	}
}
