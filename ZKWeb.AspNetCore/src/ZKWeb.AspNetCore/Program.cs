using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using ZKWeb.Testing;
using ZKWeb.Testing.TestEventHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.PlatformAbstractions;
using ZKWeb.AspNetCore.Hosting;
using ZKWebStandard.Ioc;

namespace ZKWeb.AspNetCore {
	/// <summary>
	/// 主程序入口点
	/// </summary>
	public class Program {
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
		/// 获取网站根目录
		/// </summary>
		/// <returns></returns>
		private static string GetWebsiteRootDirectory() {
			var path = PlatformServices.Default.Application.ApplicationBasePath;
			while (!File.Exists(Path.Combine(path, "Web.config"))) {
				path = Path.GetDirectoryName(path);
				if (string.IsNullOrEmpty(path)) {
					throw new DirectoryNotFoundException("Website root directory not found");
				}
			}
			return path;
		}

		/// <summary>
		/// 运行测试
		/// </summary>
		private static void RunTests() {
			// 初始化程序
			Application.Initialize(GetWebsiteRootDirectory());
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
			// 配置托管
			// 支持IIS和Kestrel
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseIISIntegration()
				.Configure(app => {
					// 初始化程序
					Application.Ioc.RegisterMany<CoreWebsiteStopper>(ReuseType.Singleton);
					Application.Initialize(GetWebsiteRootDirectory());
					// 设置处理请求的函数
					// 处理会在线程池中运行
					app.Run(coreContext => Task.Run(() => {
						var context = new CoreHttpContextWrapper(coreContext);
						try {
							// 处理请求
							Application.OnRequest(context);
						} catch (CoreHttpResponseEndException) {
							// 正常处理完毕
						} catch (Exception ex) {
							// 处理错误
							try {
								Application.OnError(context, ex);
							} catch (CoreHttpResponseEndException) {
								// 错误处理完毕
							} catch (Exception) {
								// 错误处理失败
							}
						}
					}));
				})
				.Build();
			Application.Ioc.RegisterInstance<IWebHost>(host);
			host.Run();
		}
	}
}
