using System.Collections.Generic;
using System.Linq;
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
				Assert.Equals(instance.InterfaceService.GetType(), typeof(TransientImplementation));
				Assert.Equals(instance.TestResolveFailed, false);
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

		[ExportMany]
		public class TestResolveFromConstructor {
			public IEnumerable<ClassService> ClassServices { get; set; }
			public InterfaceService InterfaceService { get; set; }
			public bool? TestResolveFailed { get; set; }

			public TestResolveFromConstructor(
				IEnumerable<ClassService> classServices,
				InterfaceService interfaceService,
				bool testResolveFailed) {
				ClassServices = classServices;
				InterfaceService = interfaceService;
				TestResolveFailed = testResolveFailed;
			}
		}
	}
}
