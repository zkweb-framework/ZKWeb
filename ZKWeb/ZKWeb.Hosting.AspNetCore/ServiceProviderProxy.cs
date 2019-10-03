using System;
using ZKWebStandard.Ioc;
using ZKWebStandard.Ioc.Extensions;

namespace ZKWeb.Hosting.AspNetCore
{
    /// <summary>
    /// Serivce provider that always use Application.Ioc as upstream for reloading support<br/>
    /// 服务提供器的代理类，总是使用 Application.Ioc 作为上游，用于支持重新加载应用<br/>
    /// </summary>
    public class ServiceProviderProxy : IServiceProvider
    {
        private volatile IContainer _container;
        private volatile IServiceProvider _upstream;

        /// <summary>
        /// Resolve service<br/>
        /// 解决服务<br/>
        /// </summary>
        public object GetService(Type serviceType)
        {
            var container = Application.Ioc;
            if (container != _container)
            {
                _upstream = container.AsServiceProvider();
                _container = container;
            }
            return _upstream.GetService(serviceType);
        }
    }
}

