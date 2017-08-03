using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZKWebStandard.Ioc;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.IocContainer {
	[Tests]
	class ContainerTest {
		public void RegisterSelf() {
			using (IContainer container = new Container()) {
				Assert.IsTrue(object.ReferenceEquals(container.Resolve<IContainer>(), container));
				Assert.IsTrue(object.ReferenceEquals(container.Resolve<IRegistrator>(), container));
				Assert.IsTrue(object.ReferenceEquals(container.Resolve<IGenericRegistrator>(), container));
				Assert.IsTrue(object.ReferenceEquals(container.Resolve<IResolver>(), container));
				Assert.IsTrue(object.ReferenceEquals(container.Resolve<IGenericResolver>(), container));
				using (IContainer clone = (IContainer)container.Clone()) {
					Assert.IsTrue(!object.ReferenceEquals(clone, container));
					Assert.IsTrue(object.ReferenceEquals(clone.Resolve<IContainer>(), clone));
					Assert.IsTrue(object.ReferenceEquals(clone.Resolve<IRegistrator>(), clone));
					Assert.IsTrue(object.ReferenceEquals(clone.Resolve<IGenericRegistrator>(), clone));
					Assert.IsTrue(object.ReferenceEquals(clone.Resolve<IResolver>(), clone));
					Assert.IsTrue(object.ReferenceEquals(clone.Resolve<IGenericResolver>(), clone));
				}
			}
		}

		public void Register() {
			using (var container = new Container()) {
				container.Register(
					typeof(InterfaceService), typeof(TransientImplementation), ReuseType.Transient, null);
				container.Register<InterfaceService, SingletonImplementation>(ReuseType.Singleton, null);
				var instances = container.ResolveMany<InterfaceService>(null).ToList();
				Assert.Equals(instances.Count, 2);
				Assert.Equals(instances[0].Name, "Transient");
				Assert.Equals(instances[1].Name, "Singleton");
				var instancesAgain = container.ResolveMany<InterfaceService>(null).ToList();
				Assert.Equals(instancesAgain.Count, 2);
				Assert.IsTrue(!object.ReferenceEquals(instances[0], instancesAgain[0]));
				Assert.IsTrue(object.ReferenceEquals(instances[1], instancesAgain[1]));
			}
		}

		public void RegisterMany() {
			using (var container = new Container()) {
				container.RegisterMany(typeof(TransientImplementation), ReuseType.Transient, "a", false);
				container.RegisterMany<SingletonImplementation>(ReuseType.Singleton, "b", false);
				Assert.Equals(
					container.Resolve<TransientImplementation>(IfUnresolved.Throw, "a").Name, "Transient");
				Assert.Equals(
					container.Resolve<ClassService>(IfUnresolved.Throw, "a").GetName(), "Transient");
				Assert.Equals(
					container.Resolve<InterfaceService>(IfUnresolved.Throw, "a").Name, "Transient");
				Assert.Equals(
					container.Resolve<SingletonImplementation>(IfUnresolved.Throw, "b").Name, "Singleton");
				Assert.Equals(
					container.Resolve<ClassService>(IfUnresolved.Throw, "b").GetName(), "Singleton");
				Assert.Equals(
					container.Resolve<InterfaceService>(IfUnresolved.Throw, "b").Name, "Singleton");
				Assert.IsTrue(object.ReferenceEquals(
					container.Resolve<SingletonImplementation>(IfUnresolved.Throw, "b"),
					container.Resolve<ClassService>(IfUnresolved.Throw, "b")));
				Assert.IsTrue(object.ReferenceEquals(
					container.Resolve<ClassService>(IfUnresolved.Throw, "b"),
					container.Resolve<InterfaceService>(IfUnresolved.Throw, "b")));
			}
		}

		public void RegisterInstance() {
			using (var container = new Container()) {
				var instance = new SingletonImplementation();
				container.RegisterInstance(typeof(InterfaceService), instance, null);
				container.RegisterInstance(typeof(ClassService), instance, null);
				Assert.IsTrue(object.ReferenceEquals(instance,
					container.Resolve<InterfaceService>(IfUnresolved.Throw, null)));
				Assert.IsTrue(object.ReferenceEquals(instance,
					container.Resolve<ClassService>(IfUnresolved.Throw, null)));
			}
		}

		public void RegisterDelegate() {
			using (var container = new Container()) {
				container.RegisterDelegate(typeof(InterfaceService),
					() => new TransientImplementation(), ReuseType.Transient, "abc");
				container.RegisterDelegate<ClassService>(
					() => new TransientImplementation(), ReuseType.Transient, "abc");
				Assert.Equals(
					container.Resolve<InterfaceService>(IfUnresolved.Throw, "abc").Name, "Transient");
				Assert.Equals(
					container.Resolve<ClassService>(IfUnresolved.Throw, "abc").GetName(), "Transient");
				Assert.IsTrue(!object.ReferenceEquals(
					container.Resolve<InterfaceService>(IfUnresolved.Throw, "abc"),
					container.Resolve<ClassService>(IfUnresolved.Throw, "abc")));
			}
		}

		public void RegisterExports() {
			using (var container = new Container()) {
				container.RegisterExports(new[] {
					typeof(TransientImplementation),
					typeof(SingletonImplementation)
				});
				Assert.Equals(
					container.Resolve<TransientImplementation>(IfUnresolved.ReturnDefault, "a"), null);
				Assert.Equals(
					container.Resolve<ClassService>(IfUnresolved.Throw, "a").GetName(), "Transient");
				Assert.Equals(
					container.Resolve<InterfaceService>(IfUnresolved.Throw, "a").Name, "Transient");
				Assert.Equals(
					container.Resolve<SingletonImplementation>(IfUnresolved.Throw, "b").Name, "Singleton");
				Assert.Equals(
					container.Resolve<ClassService>(IfUnresolved.Throw, "b").GetName(), "Singleton");
				Assert.Equals(
					container.Resolve<InterfaceService>(IfUnresolved.ReturnDefault, null), null);
				Assert.IsTrue(object.ReferenceEquals(
					container.Resolve<SingletonImplementation>(IfUnresolved.Throw, "b"),
					container.Resolve<ClassService>(IfUnresolved.Throw, "b")));
			}
		}

		public void RegisterExportsWithClearExists() {
			using (var container = new Container()) {
				container.RegisterExports(new[] {
					typeof(SingletonImplementation),
					typeof(ReplaceImplementation)
				});
				Assert.Equals(((ClassService)container.Resolve(
					typeof(ClassService), IfUnresolved.Throw, "b")).GetName(), "Replace");
			}
		}

		public void RegisterSingleExport() {
			using (var container = new Container()) {
				container.RegisterExports(new[] { typeof(SingleExportImplementation) });
				Assert.Equals(((InterfaceService)container.Resolve(
					typeof(InterfaceService), IfUnresolved.Throw, null)).Name, "SingleExport");
				Assert.Equals(((ClassService)container.Resolve(
					typeof(ClassService), IfUnresolved.Throw, "a")).GetName(), "SingleExport");
				Assert.Equals(((SingleExportImplementation)container.Resolve(
					typeof(SingleExportImplementation), IfUnresolved.ReturnDefault, null)), null);
			}
		}

		public void Unregister() {
			using (var container = new Container()) {
				container.RegisterExports(new[] { typeof(TransientImplementation) });
				Assert.Equals(
					container.Resolve<ClassService>(IfUnresolved.Throw, "a").GetName(), "Transient");
				container.Unregister<ClassService>("a");
				Assert.Equals(
					container.Resolve<ClassService>(IfUnresolved.ReturnDefault, "a"), null);
			}
		}

		public void UnregisterImplementation() {
			using (var container = new Container()) {
				container.RegisterMany<TransientImplementation>(ReuseType.Transient, null, false);
				container.RegisterMany<SingletonImplementation>(ReuseType.Singleton, null, false);
				container.UnregisterImplementation<TransientImplementation>(null);
				Assert.Equals(
					container.Resolve<ClassService>(IfUnresolved.Throw, null).GetName(), "Singleton");
				Assert.Equals(
					container.Resolve<InterfaceService>(IfUnresolved.Throw, null).Name, "Singleton");
				Assert.Equals(
					container.Resolve<TransientImplementation>(IfUnresolved.ReturnDefault, null), null);
			}
		}

		public void UnregisterAll() {
			using (var container = new Container()) {
				container.RegisterExports(new[] {
					typeof(TransientImplementation),
					typeof(SingletonImplementation)
				});
				container.UnregisterAll();
				Assert.Equals(container.Resolve<ClassService>(IfUnresolved.ReturnDefault, "a"), null);
				Assert.Equals(container.Resolve<ClassService>(IfUnresolved.ReturnDefault, "b"), null);
			}
		}

		public void Resolve() {
			using (var container = new Container()) {
				container.RegisterExports(new[] {
					typeof(TransientImplementation),
					typeof(SingletonImplementation)
				});
				container.Register<InterfaceService, TransientImplementation>(ReuseType.Transient, null);
				container.Register<InterfaceService, SingletonImplementation>(ReuseType.Singleton, null);
				Assert.Equals(((ClassService)container.Resolve(
					typeof(ClassService), IfUnresolved.Throw, "a")).GetName(), "Transient");
				Assert.Equals(((ClassService)container.Resolve(
					typeof(ClassService), IfUnresolved.Throw, "b")).GetName(), "Singleton");
				Assert.Throws<KeyNotFoundException>(() => {
					container.Resolve(typeof(ClassService), IfUnresolved.Throw, "c");
				});
				Assert.Throws<KeyNotFoundException>(() => {
					container.Resolve(typeof(InterfaceService), IfUnresolved.Throw, null);
				});
				Assert.Equals(container.Resolve(typeof(ClassService), IfUnresolved.ReturnDefault, "c"), null);
				Assert.Equals(container.Resolve(typeof(InterfaceService), IfUnresolved.ReturnDefault, null), null);
				Assert.Equals(
					container.Resolve<ClassService>(IfUnresolved.Throw, "a").GetName(), "Transient");
				Assert.Equals(
					container.Resolve<ClassService>(IfUnresolved.Throw, "b").GetName(), "Singleton");
				Assert.Throws<KeyNotFoundException>(() => {
					container.Resolve<ClassService>(IfUnresolved.Throw, "c");
				});
				Assert.Throws<KeyNotFoundException>(() => {
					container.Resolve<InterfaceService>(IfUnresolved.Throw, null);
				});
				Assert.Equals(container.Resolve<ClassService>(IfUnresolved.ReturnDefault, "c"), null);
				Assert.Equals(container.Resolve<InterfaceService>(IfUnresolved.ReturnDefault, null), null);
			}
		}

		public void ResolveMany() {
			using (var container = new Container()) {
				container.Register<InterfaceService, TransientImplementation>(ReuseType.Transient, null);
				container.Register<InterfaceService, SingletonImplementation>(ReuseType.Singleton, null);
				var instances = container.ResolveMany(typeof(InterfaceService), null).ToList();
				Assert.IsTrue(instances.Count == 2);
				Assert.Equals(((InterfaceService)instances[0]).Name, "Transient");
				Assert.Equals(((InterfaceService)instances[1]).Name, "Singleton");
				var instancesAgain = container.ResolveMany<InterfaceService>(null).ToList();
				Assert.Equals(instancesAgain.Count, 2);
				Assert.IsTrue(!object.ReferenceEquals(instances[0], instancesAgain[0]));
				Assert.IsTrue(object.ReferenceEquals(instances[1], instancesAgain[1]));
				var instancesEmpty = container.ResolveMany<InterfaceService>("x").ToList();
				Assert.Equals(instancesEmpty.Count, 0);
			}
		}

		public void ResolveFactories() {
			using (var container = new Container()) {
				container.Register<InterfaceService, TransientImplementation>(ReuseType.Transient, null);
				container.Register<InterfaceService, SingletonImplementation>(ReuseType.Singleton, null);
				var factories = container.ResolveFactories(typeof(InterfaceService), null);
				Assert.Equals(factories.Count(), 2);
				Assert.Equals(factories.First().ImplementationTypeHint, typeof(TransientImplementation));
				Assert.Equals(factories.Last().ImplementationTypeHint, typeof(SingletonImplementation));
				factories = container.ResolveFactories<InterfaceService>(null);
				Assert.Equals(factories.Count(), 2);
				Assert.Equals(factories.First().ImplementationTypeHint, typeof(TransientImplementation));
				Assert.Equals(factories.Last().ImplementationTypeHint, typeof(SingletonImplementation));
			}
		}

		public void Clone() {
			using (var container = new Container()) {
				container.RegisterExports(new[] {
					typeof(TransientImplementation),
					typeof(SingletonImplementation)
				});
				using (var containerClone = (Container)container.Clone()) {
					containerClone.Unregister<ClassService>("a");
					containerClone.Register<ClassService, TransientImplementation>(ReuseType.Transient, "x");
					Assert.Equals(
						containerClone.Resolve<ClassService>(IfUnresolved.ReturnDefault, "a"), null);
					Assert.Equals(
						containerClone.Resolve<ClassService>(IfUnresolved.Throw, "x").GetName(), "Transient");
				}
				Assert.Equals(
					container.Resolve<ClassService>(IfUnresolved.Throw, "a").GetName(), "Transient");
				Assert.Equals(
					container.Resolve<ClassService>(IfUnresolved.ReturnDefault, "x"), null);
			}
		}

		public void ResolveFromConstructor() {
			using (IContainer container = new Container()) {
				container.Register<ClassService, TransientImplementation>();
				container.Register<ClassService, SingletonImplementation>();
				container.Register<InterfaceService, TransientImplementation>();
				container.RegisterMany<TestResolveFromConstructor>();
				var instance = container.Resolve<TestResolveFromConstructor>();
				Assert.Equals(instance.ClassServices.Count(), 2);
				Assert.IsTrue(instance.ClassServices.Any(s => s is TransientImplementation));
				Assert.IsTrue(instance.ClassServices.Any(s => s is SingletonImplementation));
				Assert.Equals(instance.ClassServicesLazy.Value.Count(), 2);
				Assert.IsTrue(instance.ClassServicesLazy.Value.Any(s => s is TransientImplementation));
				Assert.IsTrue(instance.ClassServicesLazy.Value.Any(s => s is SingletonImplementation));
				Assert.Equals(instance.ClassServicesList.Count, 2);
				Assert.IsTrue(instance.ClassServicesList.Any(s => s is TransientImplementation));
				Assert.IsTrue(instance.ClassServicesList.Any(s => s is SingletonImplementation));
				Assert.Equals(instance.InterfaceService.GetType(), typeof(TransientImplementation));
				Assert.Equals(instance.InterfaceServiceLazy.Value.GetType(), typeof(TransientImplementation));
				Assert.Equals(instance.InterfaceServiceFunc().GetType(), typeof(TransientImplementation));
				Assert.Equals(instance.TestDefaultString, "default string");
				Assert.Equals(instance.TestDefaultInt, 123);
			}
		}

		public void ResolveFromInjectConstructor() {
			using (IContainer container = new Container()) {
				container.RegisterMany<TestResolveFromInjectConstructor>();
				Assert.IsTrue(container.Resolve<TestResolveFromInjectConstructor>() != null);
			}
		}

		public void ResolveScopedService() {
			using (IContainer container = new Container()) {
				container.RegisterExports(new[] { typeof(ScopedImplementation) });
				var tasks = new List<Task>();
				var resolvedServices = new List<ScopedImplementation>();
				var taskFinished = 0;
				for (var x = 0; x < 100; ++x) {
					tasks.Add(Task.Run(async () => {
						var service = container.Resolve<InterfaceService>();
						lock (resolvedServices) {
							resolvedServices.Add((ScopedImplementation)service);
						}
						var childTask = Task.Run(async () => {
							await Task.Delay(100);
							var serviceInChildTask = container.Resolve<InterfaceService>();
							Assert.Equals(service, serviceInChildTask);
						});
						await childTask;
						var serviceAfterAwait = container.Resolve<InterfaceService>();
						Assert.Equals(service, serviceAfterAwait);
						container.ScopeFinished();
						Interlocked.Increment(ref taskFinished);
					}));
				}
				Task.WaitAll(tasks.ToArray());
				Assert.Equals(100, taskFinished);
				Assert.Equals(100, resolvedServices.Count);
				Assert.Equals(100, resolvedServices.Distinct().Count());
				Assert.IsTrue(resolvedServices.All(s => s.DisposeCount == 1));
			}
		}

		public void ResolveGenericDefinition() {
			using (IContainer container = new Container()) {
				container.Register(typeof(IGenericService<>), typeof(GenericServiceA<>));
				Assert.Equals(
					container.Resolve<IGenericService<int>>().GetType(),
					typeof(GenericServiceA<int>));
				Assert.Equals(
					container.Resolve<IGenericService<string>>().GetType(),
					typeof(GenericServiceA<string>));
			}

			using (IContainer container = new Container()) {
				container.Register(
					typeof(IGenericService<>), typeof(GenericServiceA<>), ReuseType.Singleton);
				Assert.IsTrue(object.ReferenceEquals(
					container.Resolve<IGenericService<int>>(),
					container.Resolve<IGenericService<int>>()));
				Assert.IsTrue(object.ReferenceEquals(
					container.Resolve<IGenericService<string>>(),
					container.Resolve<IGenericService<string>>()));
				Assert.IsTrue(!object.ReferenceEquals(
					container.Resolve<IGenericService<int>>(),
					container.Resolve<IGenericService<string>>()));
			}

			using (IContainer container = new Container()) {
				container.Register(typeof(IGenericService<>), typeof(GenericServiceA<>));
				container.Register(typeof(IGenericService<>), typeof(GenericServiceB<>));
				var resolved = container.ResolveMany<IGenericService<int>>().ToList();
				Assert.Equals(resolved.Count, 2);
				Assert.Equals(resolved[0].GetType(), typeof(GenericServiceA<int>));
				Assert.Equals(resolved[1].GetType(), typeof(GenericServiceB<int>));
			}
		}

		public interface InterfaceService {
			string Name { get; }
		}

		public abstract class ClassService {
			public abstract string GetName();
		}

		[ExportMany(ContractKey = "a", Except = new[] { typeof(TransientImplementation) })]
		public class TransientImplementation : ClassService, InterfaceService {
			public string Name { get { return "Transient"; } }
			public override string GetName() { return Name; }
		}

		[ExportMany(ContractKey = "b", Except = new[] { typeof(InterfaceService) })]
		[SingletonReuse]
		public class SingletonImplementation : ClassService, InterfaceService {
			public string Name { get { return "Singleton"; } }
			public override string GetName() { return Name; }
		}

		[ExportMany(ContractKey = "b", ClearExists = true)]
		public class ReplaceImplementation : ClassService, InterfaceService {
			public string Name { get { return "Replace"; } }
			public override string GetName() { return Name; }
		}

		[Export(ServiceType = typeof(InterfaceService))]
		[Export(ServiceType = typeof(ClassService), ContractKey = "a")]
		public class SingleExportImplementation : ClassService, InterfaceService {
			public string Name { get { return "SingleExport"; } }
			public override string GetName() { return Name; }
		}

		[Export(ServiceType = typeof(InterfaceService))]
		[ScopedReuse]
		public class ScopedImplementation : InterfaceService, IDisposable {
			public string Name { get { return "Scoped"; } }
			public int DisposeCount;

			public void Dispose() {
				Interlocked.Increment(ref DisposeCount);
			}
		}

		[ExportMany]
		public class TestResolveFromConstructor {
			public IEnumerable<ClassService> ClassServices { get; set; }
			public Lazy<IEnumerable<ClassService>> ClassServicesLazy { get; set; }
			public IList<ClassService> ClassServicesList { get; set; }
			public InterfaceService InterfaceService { get; set; }
			public Lazy<InterfaceService> InterfaceServiceLazy { get; set; }
			public Func<InterfaceService> InterfaceServiceFunc { get; set; }
			public string TestDefaultString { get; set; }
			public int TestDefaultInt { get; set; }

			public TestResolveFromConstructor(
				IEnumerable<ClassService> classServices,
				Lazy<IEnumerable<ClassService>> classServicesLazy,
				IList<ClassService> classServicesList,
				InterfaceService interfaceService,
				Lazy<InterfaceService> interfaceServiceLazy,
				Func<InterfaceService> interfaceServiceFunc,
				string testDefaultString = "default string",
				int testDefaultInt = 123) {
				ClassServices = classServices;
				ClassServicesLazy = classServicesLazy;
				ClassServicesList = classServicesList;
				InterfaceService = interfaceService;
				InterfaceServiceLazy = interfaceServiceLazy;
				InterfaceServiceFunc = interfaceServiceFunc;
				TestDefaultString = testDefaultString;
				TestDefaultInt = testDefaultInt;
			}
		}

		[ExportMany]
		public class TestResolveFromInjectConstructor {
			public TestResolveFromInjectConstructor(int a) {
				Assert.IsTrueWith(false, "choose wrong constructor");
			}

			[Inject]
			public TestResolveFromInjectConstructor(string b = "right") {
			}
		}

		interface IGenericService<T> { }

		class GenericServiceA<T> : IGenericService<T> { }

		class GenericServiceB<T> : IGenericService<T> { }
	}
}
