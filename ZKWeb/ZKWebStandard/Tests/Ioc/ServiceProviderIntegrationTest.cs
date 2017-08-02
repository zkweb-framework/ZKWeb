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
			Assert.IsTrue(provider.GetService<TestData>() != null);
			Assert.IsTrue(provider.GetService<Func<TestData>>() != null);
			Assert.IsTrue(provider.GetService<Lazy<TestData>>().Value != null);
			Assert.Equals(provider.GetService<List<TestData>>().Count, 1);
			Assert.Equals(provider.GetService<IList<TestData>>().Count, 1);
			Assert.Equals(provider.GetService<IEnumerable<TestData>>().Count(), 1);
			Assert.Equals(provider.GetServices<TestData>().Count(), 1);
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
			var injection = (TestInjection)provider.GetService(typeof(TestInjection));
			Assert.IsTrue(injection != null);
			Assert.IsTrue(injection.Data != null);
			var str = (string)provider.GetService(typeof(string));
			Assert.Equals(str, "abc");
			var list = provider.GetService(typeof(ITestGenericService<int, string>));
			Assert.Equals(list.GetType(), typeof(TestGenericService<int, string>));
		}

		public void ResolveMultiConstructor() {
			IContainer container = new Container();
			container.RegisterMany<TestData>(ReuseType.Singleton);
			container.RegisterMany<TestInjection>(ReuseType.Singleton);
			container.RegisterMany<TestMultiConstructor>(ReuseType.Singleton);
			var provider = container.AsServiceProvider();
			var resolved = provider.GetService<TestMultiConstructor>();
			Assert.Equals(resolved.UsedConstructor, "data+injection");
			Assert.Equals(resolved.Data, provider.GetService<TestData>());
			Assert.Equals(resolved.Injection, provider.GetService<TestInjection>());
			Assert.Equals(provider.GetService<TestMultiConstructor>(), resolved);

			container = new Container();
			container.RegisterMany<TestData>(ReuseType.Singleton);
			container.RegisterMany<TestMultiConstructor>(ReuseType.Transient);
			provider = container.AsServiceProvider();
			resolved = provider.GetService<TestMultiConstructor>();
			Assert.Equals(resolved.UsedConstructor, "data");
			Assert.Equals(resolved.Data, provider.GetService<TestData>());
			Assert.Equals(resolved.Injection, null);

			container.UnregisterImplementation<TestData>();
			Assert.Throws<KeyNotFoundException>(() => {
				provider.GetService<TestMultiConstructor>();
			});
			container.Resolve<IMultiConstructorResolver>().ClearCache();
			resolved = provider.GetService<TestMultiConstructor>();
			Assert.Equals(resolved.UsedConstructor, "empty");
			Assert.Equals(resolved.Data, null);
			Assert.Equals(resolved.Injection, null);
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

		class TestMultiConstructor {
			public TestData Data { get; set; }
			public TestInjection Injection { get; set; }
			public string UsedConstructor { get; set; }

			public TestMultiConstructor() {
				UsedConstructor = "empty";
			}

			public TestMultiConstructor(TestData data) {
				Data = data;
				UsedConstructor = "data";
			}

			public TestMultiConstructor(TestData data, TestInjection injection) {
				Data = data;
				Injection = injection;
				UsedConstructor = "data+injection";
			}
		}
	}
}
