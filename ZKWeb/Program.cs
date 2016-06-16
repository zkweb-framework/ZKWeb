using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;
using ZKWeb.Cache;
using ZKWeb.Cache.Policies;
using ZKWeb.Database;
using ZKWeb.Localize;
using ZKWeb.Localize.JsonConverters;
using ZKWeb.Logging;
using ZKWeb.Plugin;
using ZKWeb.Serialize;
using ZKWeb.Server;
using ZKWeb.Templating;
using ZKWeb.Templating.DynamicContents;
using ZKWeb.Testing;
using ZKWeb.Testing.TestEventHandlers;
using ZKWeb.Web;
using ZKWeb.Web.HttpRequestHandlers;
using ZKWebStandard.Extensions;
using ZKWebStandard.Ioc;
using ZKWebStandard.Web;
using System.Threading;
using ZKWeb.Web.Wrappers;

namespace ZKWeb {
	/// <summary>
	/// 主程序入口点
	/// </summary>
	internal static class Program {
		/// <summary>
		/// 主程序入口点
		/// </summary>
		/// <param name="args">命令行参数</param>
		public static void Main(string[] args) {
			var action = (args.Length > 0) ? args[0] : "";
			if (action == "--run_tests") {
				RunTests();
			} else if (action == "--help") {
				Console.WriteLine("The following commands are available:");
				Console.WriteLine("    zkweb.exe");
				Console.WriteLine("    zkweb.exe --run_tests");
				Console.WriteLine("    zkweb.exe --help");
			} else {
				RunWebsite();
			}
		}

		/// <summary>
		/// 初始化程序
		/// </summary>
		private static void Initialize() {
			// 注册核心组件
			var ioc = Application.Ioc;
			ioc.UnregisterAll();
			ioc.RegisterMany<DatabaseManager>(ReuseType.Singleton);
			ioc.RegisterMany<TJsonConverter>(ReuseType.Singleton);
			ioc.RegisterMany<TranslateManager>(ReuseType.Singleton);
			ioc.RegisterMany<LogManager>(ReuseType.Singleton);
			ioc.RegisterMany<PluginManager>(ReuseType.Singleton);
			ioc.RegisterMany<ConfigManager>(ReuseType.Singleton);
			ioc.RegisterMany<PathConfig>(ReuseType.Singleton);
			ioc.RegisterMany<PathManager>(ReuseType.Singleton);
			ioc.RegisterMany<TemplateAreaManager>(ReuseType.Singleton);
			ioc.RegisterMany<TemplateFileSystem>(ReuseType.Singleton);
			ioc.RegisterMany<TemplateManager>(ReuseType.Singleton);
			ioc.RegisterMany<TestManager>(ReuseType.Singleton);
			ioc.RegisterMany<AddVersionHeaderHandler>(ReuseType.Singleton);
			ioc.RegisterMany<DefaultErrorHandler>(ReuseType.Singleton);
			ioc.RegisterMany<ControllerManager>(ReuseType.Singleton);
			ioc.RegisterMany<CacheIsolateByDevice>(ReuseType.Singleton, serviceKey: "Device");
			ioc.RegisterMany<CacheIsolateByLocale>(ReuseType.Singleton, serviceKey: "Locale");
			ioc.RegisterMany<CacheIsolateByUrl>(ReuseType.Singleton, serviceKey: "Url");
			// 初始化核心组件
			ConfigManager.Initialize();
			PluginManager.Initialize();
			JsonNetInitializer.Initialize();
			TemplateManager.Initialize();
			ControllerManager.Initialize();
			DatabaseManager.Initialize();
			// 初始化所有插件并调用网站启动时的处理
			ioc.ResolveMany<IPlugin>().ForEach(p => { });
			ioc.ResolveMany<IWebsiteStartHandler>().ForEach(h => h.OnWebsiteStart());
			// 初始化常驻型的核心组件
			PluginReloader.Start();
			AutomaticCacheCleaner.Start();
		}

		/// <summary>
		/// 运行测试
		/// </summary>
		private static void RunTests() {
			// 初始化程序
			Initialize();
			// 运行所有测试
			var unitTestManager = Application.Ioc.Resolve<TestManager>();
			unitTestManager.RunAllAssemblyTest(new TestConsoleEventHandler());
			// 等待结束
			Console.WriteLine("done");
			Console.ReadLine();
		}

		/// <summary>
		/// 运行网站
		/// </summary>
		private static void RunWebsite() {
			// 请求处理函数
			var onRequest = new Action<IHttpContext>(context => {
				// 重载当前的Http上下文
				using (HttpManager.OverrideContext(context)) {
					var ioc = Application.Ioc;
					try {
						// 调用预处理器，先注册的先调用
						foreach (var handler in ioc.ResolveMany<IHttpRequestPreHandler>()) {
							handler.OnRequest();
						}
						// 调用处理器，后注册的先调用
						foreach (var handler in ioc.ResolveMany<IHttpRequestHandler>().Reverse()) {
							handler.OnRequest();
						}
						// 没有处理时返回404
						throw new HttpException(404, "Not Found");
					} catch (ThreadAbortException) {
						// 正常处理完毕
						// 重置收到的线程终止请求
						Thread.ResetAbort();
					} catch (Exception ex) {
						// 调用错误处理器，后注册的先调用
						try {
							foreach (var handler in ioc.ResolveMany<IHttpRequestErrorHandler>()) {
								handler.OnError(ex);
							}
						} catch (ThreadAbortException) {
							// 错误处理完毕
							// 重置收到的线程终止请求
							Thread.ResetAbort();
						} catch (Exception) {
							// 错误处理失败
						}
					}
				}
			});
			// 程序配置函数
			var onConfiguration = new Action<IApplicationBuilder>(app => {
				// 初始化程序
				Initialize();
				// 设置线程池使用尽可能多的线程
				// 实际本机设置的数量是(32767, 32767)
				ThreadPool.SetMaxThreads(int.MaxValue, int.MaxValue);
				// 设置处理请求的函数
				// 处理会在线程池中执行
				app.Run(context => Task.Run(() => onRequest(new CoreHttpContextWrapper(context))));
			});
			// 配置托管
			// 支持IIS和Kestrel
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseIISIntegration()
				.Configure(onConfiguration)
				.Build();
			Application.Ioc.RegisterInstance<IWebHost>(host);
			host.Run();
		}
	}
}
