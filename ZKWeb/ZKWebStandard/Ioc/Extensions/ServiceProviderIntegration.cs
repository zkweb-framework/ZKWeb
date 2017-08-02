using Microsoft.Extensions.DependencyInjection;
using System;

namespace ZKWebStandard.Ioc.Extensions {
	/// <summary>
	/// Intergration with IServiceCollection and IServiceProvider
	/// </summary>
	public static class ServiceProviderIntegration {
		/// <summary>
		/// Construct an IServiceProvider adapter<br/>
		/// 构建一个IServiceProvider转接器<br/>
		/// </summary>
		/// <param name="container">IoC container</param>
		/// <returns></returns>
		public static IServiceProvider AsServiceProvider(this IContainer container) {
			var provider = container.Resolve<IServiceProvider>(IfUnresolved.ReturnDefault);
			// Register IServiceProvider
			if (provider == null) {
				provider = new ServiceProviderAdapter(container);
				container.RegisterInstance<IServiceProvider>(provider);
			}
			// Register IServiceScopeFactory
			if (container.Resolve<IServiceScopeFactory>(IfUnresolved.ReturnDefault) == null) {
				container.RegisterInstance<IServiceScopeFactory>(
					new ServiceScopeFactory(container));
			}
			// Register IMultiConstructorResolver
			if (container.Resolve<IMultiConstructorResolver>(IfUnresolved.ReturnDefault) == null) {
				container.RegisterInstance<IMultiConstructorResolver>(
					new MultiConstructorResolver(container));
			}
			return provider;
		}

		/// <summary>
		/// Register services from IServiceCollection<br/>
		/// 从IServiceCollection注册服务<br/>
		/// </summary>
		/// <param name="container">IoC container</param>
		/// <param name="serviceCollection">Service collection</param>
		public static void RegisterFromServiceCollection(
			this IContainer container, IServiceCollection serviceCollection) {
			var provider = container.AsServiceProvider();
			foreach (var descriptor in serviceCollection) {
				// Convert ReuseType
				ReuseType reuseType;
				if (descriptor.Lifetime == ServiceLifetime.Transient) {
					reuseType = ReuseType.Transient;
				} else if (descriptor.Lifetime == ServiceLifetime.Singleton) {
					reuseType = ReuseType.Singleton;
				} else if (descriptor.Lifetime == ServiceLifetime.Scoped) {
					reuseType = ReuseType.Scoped;
				} else {
					throw new NotSupportedException($"Unsupported ServiceLifetime: {descriptor.Lifetime}");
				}
				// Register service
				if (descriptor.ImplementationType != null) {
					container.Register(descriptor.ServiceType,
						descriptor.ImplementationType, reuseType, null);
				} else if (descriptor.ImplementationFactory != null) {
					container.RegisterDelegate(descriptor.ServiceType,
						() => descriptor.ImplementationFactory(provider), reuseType, null);
				} else {
					container.RegisterInstance(
						descriptor.ServiceType, descriptor.ImplementationInstance, null);
				}
			}
		}
	}
}
