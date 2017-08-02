using Microsoft.Extensions.DependencyInjection;
using System;

namespace ZKWebStandard.Ioc.Extensions {
	/// <summary>
	/// Service scope factory<br/>
	/// 服务范围的工厂类<br/>
	/// </summary>
	public class ServiceScopeFactory : IServiceScopeFactory {
		/// <summary>
		/// Container<br/>
		/// 容器<br/>
		/// </summary>
		protected IContainer Container { get; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public ServiceScopeFactory(IContainer container) {
			Container = container;
		}

		/// <summary>
		/// Create service scope<br/>
		/// 创建服务范围<br/>
		/// </summary>
		public IServiceScope CreateScope() {
			return new ServiceScope(Container);
		}

		/// <summary>
		/// Service scope<br/>
		/// 服务范围<br/>
		/// </summary>
		private class ServiceScope : IServiceScope {
			public IContainer Container { get; }
			public IServiceProvider ServiceProvider { get; }

			public ServiceScope(IContainer container) {
				Container = container;
				ServiceProvider = container.AsServiceProvider();
			}

			public void Dispose() {
				Container.ScopeFinished();
			}
		}
	}
}
