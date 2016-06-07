using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.IocContainer;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.IocContainer {
	[UnitTest]
	class ContainerTest {
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

		public interface InterfaceService {
			string Name { get; }
		}

		public abstract class ClassService {
			public abstract string GetName();
		}

		[Export("a", typeof(ClassService))]
		[Export("a", typeof(InterfaceService))]
		public class TransientImplementation : ClassService, InterfaceService {
			public string Name { get { return "Transient"; } }
			public override string GetName() { return Name; }
		}

		[ExportManyAttribute(ContractKey = "b", Except = new[] { typeof(InterfaceService) })]
		[SingletonReuse]
		public class SingletonImplementation : ClassService, InterfaceService {
			public string Name { get { return "Singleton"; } }
			public override string GetName() { return Name; }
		}
	}
}
