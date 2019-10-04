using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using ZKWeb;
using ZKWeb.Hosting.AspNetCore;
using ZKWeb.Server;
using ZKWebStandard.Extensions;
using ZKWebStandard.Ioc;
using ZKWebStandard.Ioc.Extensions;
using ZKWebStandard.Web;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Asp.net Core application builder extension functions<br/>
    /// Asp.Net Core应用构建器的扩展函数<br/>
    /// </summary>
    public static class CoreBuilderExtensions
    {
        /// <summary>
        /// Add zkweb services<br/>
        /// 添加ZKWeb服务<br/>
        /// </summary>
        public static IServiceProvider AddZKWeb(
            this IServiceCollection services, string websiteRootDirectory)
        {
            return services.AddZKWeb<DefaultApplication>(websiteRootDirectory);
        }

        /// <summary>
        /// Add zkweb services<br/>
        /// 添加ZKWeb服务<br/>
        /// </summary>
        public static IServiceProvider AddZKWeb<TApplication>(
            this IServiceCollection services, string websiteRootDirectory)
            where TApplication : IApplication, new()
        {
            Application.Initialize(() =>
            {
                var application = new TApplication();
                application.Ioc.RegisterMany<CoreWebsiteStopper>(ReuseType.Singleton);
                application.Ioc.RegisterFromServiceCollection(services);
                return application;
            },
            websiteRootDirectory);
            return new ServiceProviderProxy();
        }

        /// <summary>
        /// Use zkweb middleware<br/>
        /// 使用zkweb中间件<br/>
        /// </summary>
        [DebuggerNonUserCode]
        public static IApplicationBuilder UseZKWeb(this IApplicationBuilder app)
        {
            // It can't throw any exception otherwise application will get killed
            var hostingEnvironment = app.ApplicationServices.GetService<IHostingEnvironment>();
            var isDevelopment = hostingEnvironment.IsDevelopment();
            return app.Use((coreContext, next) => Task.Run(() =>
            {
                var context = new CoreHttpContextWrapper(coreContext);
                // Get application instance at first for reloading support
                IApplication instance;
                try
                {
                    instance = Application.Instance;
                }
                catch (Exception ex)
                {
                    // Initialize application failed
                    coreContext.Response.StatusCode = 500;
                    coreContext.Response.ContentType = "text/plain;charset=utf-8";
                    using (var writer = new StreamWriter(coreContext.Response.Body))
                    {
                        if (isDevelopment)
                            writer.Write(ex.ToDetailedString());
                        else
                            writer.Write(
                                "Internal error occurs during application initialization, " +
                                "please set ASPNETCORE_ENVIRONMENT to Development to view the error message, " +
                                "or check the logs on server.\r\n");
                        writer.Flush();
                    }
                    return;
                }
                try
                {
                    // Handle request
                    instance.OnRequest(context);
                }
                catch (CoreHttpResponseEndException)
                {
                    // Success
                }
                catch (Exception ex)
                {
                    // Error
                    if (ex is HttpException && ((HttpException)ex).StatusCode == 404)
                    {
                        // Try next middleware
                        try
                        {
                            next().Wait();
                            return;
                        }
                        catch (Exception nextEx)
                        {
                            ex = nextEx;
                        }
                    }
                    try
                    {
                        instance.OnError(context, ex);
                    }
                    catch (CoreHttpResponseEndException)
                    {
                        // Handle error success
                    }
                    catch (Exception)
                    {
                        // Handle error failed
                    }
                }
            }));
        }
    }
}
