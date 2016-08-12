using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
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
			// Prepare database migration
			IModel initialModel = null;
			using (var context = new EFCoreDatabaseContextBase(Database, ConnectionString)) {
				// We may need create a new database and migration history table
				// It's done here
				context.Database.EnsureCreated();
				initialModel = context.Model;
			}
			// Perform database migration
			using (var context = new EFCoreDatabaseContext(Database, ConnectionString)) {
				var serviceProvider = ((IInfrastructure<IServiceProvider>)context).Instance;
				var databaseCreator = serviceProvider.GetService<IDatabaseCreator>();
				if (databaseCreator is IRelationalDatabaseCreator) {
					// It's a relational database, create and apply the migration
					MigrateRelationalDatabase(context, initialModel);
				} else {
					// It maybe an in-memory database or no-sql database, do nothing
				}
			}
		}

		/// <summary>
		/// Create and apply the migration for relational database
		/// See: https://github.com/aspnet/EntityFramework/blob/master/src/Microsoft.EntityFrameworkCore.Relational/Storage/RelationalDatabaseCreator.cs
		/// </summary>
		/// <param name="context">Entity Framework Core database context</param>
		/// <param name="initialModel">Initial model, only contains migration history</param>
		private void MigrateRelationalDatabase(DbContext context, IModel initialModel) {
			var serviceProvider = ((IInfrastructure<IServiceProvider>)context).Instance;
			// Get the last migration model
			var histories = context.Set<EFCoreMigrationHistory>();
			var lastMigration = histories.OrderByDescending(h => h.Revision).FirstOrDefault();
			var lastModel = initialModel;
			// TODO: deserialize the model


			// Compare with the newest model
			var modelDiffer = serviceProvider.GetService<IMigrationsModelDiffer>();
			var sqlGenerator = serviceProvider.GetService<IMigrationsSqlGenerator>();
			var commandExecutor = serviceProvider.GetService<IMigrationCommandExecutor>();
			var operations = modelDiffer.GetDifferences(lastModel, context.Model);
			if (operations.Count <= 0) {
				// There no difference
				return;
			}
			// There some difference, we need perform the migration
			var commands = sqlGenerator.Generate(operations, context.Model);
			var connection = serviceProvider.GetService<IRelationalConnection>();
			// insert the history first, if migration failed, delete it
			// TODO: serialize the model
			var history = new EFCoreMigrationHistory("Test");
			histories.Add(history);
			context.SaveChanges();
			try {
				// Execute migration commands
				commandExecutor.ExecuteNonQuery(commands, connection);
			} catch {
				histories.Remove(history);
				context.SaveChanges();
				throw;
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
