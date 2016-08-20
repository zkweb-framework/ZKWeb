using ZKWebStandard.Extensions;
using ZKWebStandard.Ioc;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Extensions {
	[Tests]
	class IContainerTest {
		public void BuildFactoryWithFactory() {
			var container = new Container();
			var factoryA = container.BuildFactory(() => new TestData(), ReuseType.Transient);
			Assert.IsTrue(!object.ReferenceEquals(factoryA(), factoryA()));
			var factoryB = container.BuildFactory(() => new TestData(), ReuseType.Singleton);
			Assert.IsTrue(object.ReferenceEquals(factoryB(), factoryB()));
		}

		public void BuildFactoryWithType() {
			IContainer container = new Container();
			container.RegisterMany<TestData>();
			var factoryA = container.BuildFactory(typeof(TestInjection), ReuseType.Transient);
			var testInjectionA = (TestInjection)factoryA();
			Assert.IsTrueWith(testInjectionA.Data != null, testInjectionA);
			Assert.IsTrue(!object.ReferenceEquals(testInjectionA, factoryA()));
			var factoryB = container.BuildFactory(typeof(TestInjection), ReuseType.Singleton);
			var testInjectionB = (TestInjection)factoryB();
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
