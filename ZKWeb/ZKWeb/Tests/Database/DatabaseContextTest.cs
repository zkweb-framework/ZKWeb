using System;
using System.Collections.Generic;
using System.Linq;
using ZKWeb.Database;
using ZKWeb.Testing;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;

namespace ZKWeb.Tests.Database {
	[Tests]
	class DatabaseContextTest {
		public void All() {
			var testManager = Application.Ioc.Resolve<TestManager>();
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<IEntity>();
				Application.Ioc.RegisterMany<TestTable>();
				using (testManager.UseTemporaryDatabase()) {
					var databaseManager = Application.Ioc.Resolve<DatabaseManager>();
					var handler = new TestTableOperationHandler();
					Application.Ioc.RegisterInstance<IEntityOperationHandler<TestTable>>(handler);
					// Save
					using (var context = databaseManager.CreateContext()) {
						var obj = new TestTable() { Name = "TestName" };
						var otherObj = new TestTable() { Name = "OtherName" };
						context.Save(ref obj);
						context.Save(ref otherObj);
						Assert.Equals(handler.BeforeSaveCount, 2);
						Assert.Equals(handler.AfterSaveCount, 2);
					}
					// BatchUpdate
					using (var context = databaseManager.CreateContext()) {
						context.BatchUpdate<TestTable>(
							t => t.Name == "TestName",
							t => {
								t.Name = "TestNameUpdated";
								t.Extra["TestKey"] = "TestValue";
							});
						Assert.Equals(handler.BeforeSaveCount, 3);
						Assert.Equals(handler.AfterSaveCount, 3);
					}
					// Query
					using (var context = databaseManager.CreateContext()) {
						var obj = context.Get<TestTable>(t => t.Name == "TestNameUpdated");
						Assert.IsTrue(obj != null);
						Assert.IsTrue(obj.Extra != null);
						Assert.Equals(obj.Extra.GetOrDefault("TestKey"), "TestValue");
						var objFromQuery = context.Query<TestTable>()
							.FirstOrDefault(t => t.Name == "TestNameUpdated");
						Assert.Equals(obj, objFromQuery);
					}
					// Count
					using (var context = databaseManager.CreateContext()) {
						Assert.Equals(context.Count<TestTable>(_ => true), 2);
						Assert.Equals(context.Count<TestTable>(t => t.Name == "TestNameUpdated"), 1);
						Assert.Equals(context.Count<TestTable>(t => t.Name == "NotExist"), 0);
					}
					// BatchDelete
					using (var context = databaseManager.CreateContext()) {
						context.BatchDelete<TestTable>(t => t.Name == "TestNameUpdated");
						Assert.IsTrue(context.Get<TestTable>(t => t.Name == "TestNameUpdated") == null);
						Assert.Equals(handler.BeforeDeleteCount, 1);
						Assert.Equals(handler.AfterDeleteCount, 1);
					}
					// Get
					using (var context = databaseManager.CreateContext()) {
						Assert.IsTrue(context.Get<TestTable>(t => t.Name == "TestNameUpdated") == null);
						Assert.IsTrue(context.Get<TestTable>(t => t.Name == "OtherName") != null);
					}
					// BatchSave, BeginTransaction, CommitTransaction
					using (var context = databaseManager.CreateContext()) {
						context.BeginTransaction();
						context.BeginTransaction();
						var objs = new[] {
							new TestTable() { Name = "TestName_1" },
							new TestTable() { Name = "TestName_2" },
							new TestTable() { Name = "TestName_3" }
						}.AsEnumerable();
						context.BatchSave(ref objs);
						context.FinishTransaction();
						context.FinishTransaction();
					}
					using (var context = databaseManager.CreateContext()) {
						context.BeginTransaction();
						context.BeginTransaction();
						Assert.IsTrue(context.Get<TestTable>(t => t.Name == "TestName_1") != null);
						Assert.IsTrue(context.Get<TestTable>(t => t.Name == "TestName_2") != null);
						Assert.IsTrue(context.Get<TestTable>(t => t.Name == "TestName_3") != null);
						// only begin, no finish
					}
				}
			}
		}

		public void FastBatchActions() {
			var testManager = Application.Ioc.Resolve<TestManager>();
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<IEntity>();
				Application.Ioc.RegisterMany<TestTable>();
				using (testManager.UseTemporaryDatabase()) {
					var databaseManager = Application.Ioc.Resolve<DatabaseManager>();
					// FastBatchSave
					using (var context = databaseManager.CreateContext()) {
						var objs = new[] {
							new TestTable() { Id = Guid.NewGuid(), Name = "TestName_1" },
							new TestTable() { Id = Guid.NewGuid(), Name = "TestName_2" },
							new TestTable() { Id = Guid.NewGuid(), Name = "TestName_3" }
						};
						context.FastBatchSave<TestTable, Guid>(objs);
					}
					using (var context = databaseManager.CreateContext()) {
						Assert.IsTrue(context.Get<TestTable>(t => t.Name == "TestName_1") != null);
						Assert.IsTrue(context.Get<TestTable>(t => t.Name == "TestName_2") != null);
						Assert.IsTrue(context.Get<TestTable>(t => t.Name == "TestName_3") != null);
					}
					// FastBatchDelete
					using (var context = databaseManager.CreateContext()) {
						context.FastBatchDelete<TestTable, Guid>(a => a.Name.StartsWith("TestName_"));
					}
					using (var context = databaseManager.CreateContext()) {
						Assert.Equals(context.Get<TestTable>(t => t.Name == "TestName_1"), null);
						Assert.Equals(context.Get<TestTable>(t => t.Name == "TestName_2"), null);
						Assert.Equals(context.Get<TestTable>(t => t.Name == "TestName_3"), null);
					}
				}
			}
		}

		class TestTable : IEntity<Guid>, IEntityMappingProvider<TestTable> {
			public virtual Guid Id { get; set; }
			public virtual string Name { get; set; }
			public virtual Dictionary<string, object> Extra { get; set; }

			public TestTable() {
				Extra = new Dictionary<string, object>();
			}

			public virtual void Configure(IEntityMappingBuilder<TestTable> builder) {
				builder.Id(t => t.Id);
				builder.Map(t => t.Name);
				builder.Map(t => t.Extra, new EntityMappingOptions() { WithSerialization = true });
			}
		}

		class TestTableOperationHandler : IEntityOperationHandler<TestTable> {
			public int BeforeSaveCount { get; set; }
			public int AfterSaveCount { get; set; }
			public int BeforeDeleteCount { get; set; }
			public int AfterDeleteCount { get; set; }

			public void BeforeSave(IDatabaseContext context, TestTable data) {
				++BeforeSaveCount;
			}

			public void AfterSave(IDatabaseContext context, TestTable data) {
				++AfterSaveCount;
			}

			public void BeforeDelete(IDatabaseContext context, TestTable data) {
				++BeforeDeleteCount;
			}

			public void AfterDelete(IDatabaseContext context, TestTable data) {
				++AfterDeleteCount;
			}
		}
	}
}
