using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using ZKWebStandard.Ioc;
using ZKWebStandard.Ioc.Extensions;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Ioc {
	[Tests]
	class ServiceProviderIntegrationTest {
		public void AsServiceProvider() {
			IContainer container = new Container();
			container.RegisterMany<TestData>();
			var provider = container.AsServiceProvider();
			Assert.Equals(provider, container.AsServiceProvider());
			Assert.Equals(provider, container.Resolve<IServiceProvider>());
			Assert.IsTrue(provider.GetService(typeof(TestData)) != null);
			Assert.IsTrue(((Func<TestData>)provider.GetService(typeof(Func<TestData>)))() != null);
			Assert.IsTrue(((Lazy<TestData>)provider.GetService(typeof(Lazy<TestData>))).Value != null);
			Assert.Equals(((List<TestData>)provider.GetService(typeof(List<TestData>))).Count, 1);
			Assert.Equals(((IList<TestData>)provider.GetService(typeof(IList<TestData>))).Count, 1);
			Assert.Equals(((IEnumerable<TestData>)provider.GetService(typeof(IEnumerable<TestData>))).Count(), 1);
		}

		public void RegisterFromServices() {
			var serviceCollection = new TestServiceCollection();
			serviceCollection.Add(new ServiceDescriptor(
				typeof(TestData), _ => new TestData(), ServiceLifetime.Transient));
			serviceCollection.Add(new ServiceDescriptor(
				typeof(TestInjection), typeof(TestInjection), ServiceLifetime.Transient));
			serviceCollection.Add(new ServiceDescriptor(typeof(string), "abc"));
			serviceCollection.Add(new ServiceDescriptor(
				typeof(ITestGenericService<,>), typeof(TestGenericService<,>), ServiceLifetime.Transient));
			IContainer container = new Container();
			container.RegisterFromServiceCollection(serviceCollection);
			var provider = container.AsServiceProvider();
			var injection = (TestInjection)provider.GetRequiredService(typeof(TestInjection));
			Assert.IsTrue(injection != null);
			Assert.IsTrue(injection.Data != null);
			var str = (string)provider.GetRequiredService(typeof(string));
			Assert.Equals(str, "abc");
			var list = provider.GetRequiredService(typeof(ITestGenericService<int, string>));
			Assert.Equals(list.GetType(), typeof(TestGenericService<int, string>));
		}

		class TestData { }

		class TestInjection {
			public TestData Data { get; set; }
			public TestInjection(TestData data) {
				Data = data;
			}
		}

		class TestServiceCollection : List<ServiceDescriptor>, IServiceCollection { }

		interface ITestGenericService<Key, Value> { }

		class TestGenericService<Key, Value> : ITestGenericService<Key, Value> { }
	}
}
