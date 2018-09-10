using ZKWebStandard.Ioc;
using ZKWebStandard.Ioc.Extensions;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Ioc {
	[Tests]
	class ContainerFactoryBuilderTest {
		public void GenericWrapFactory() {
			var container = new Container();
			var factoryA = container.GenericWrapFactory(() => new TestData(), ReuseType.Transient);
			Assert.IsTrue(!object.ReferenceEquals(factoryA(), factoryA()));
			var factoryB = container.GenericWrapFactory(() => new TestData(), ReuseType.Singleton);
			Assert.IsTrue(object.ReferenceEquals(factoryB(), factoryB()));
		}

		public void GenericBuildAndWrapFactory() {
			IContainer container = new Container();
			container.RegisterMany<TestData>();
			var factoryA = container.GenericBuildAndWrapFactory<TestInjection>(ReuseType.Transient);
			var testInjectionA = factoryA();
			Assert.IsTrueWith(testInjectionA.Data != null, testInjectionA);
			Assert.IsTrue(!object.ReferenceEquals(testInjectionA, factoryA()));
			var factoryB = container.GenericBuildAndWrapFactory<TestInjection>(ReuseType.Singleton);
			var testInjectionB = factoryB();
			Assert.IsTrueWith(testInjectionB.Data != null, testInjectionB);
			Assert.IsTrue(object.ReferenceEquals(testInjectionB, factoryB()));
		}

		class TestData { }

		class TestInjection {
			public TestData Data { get; set; }
			public TestInjection(TestData data) {
				Data = data;
			}
		}
	}
}
