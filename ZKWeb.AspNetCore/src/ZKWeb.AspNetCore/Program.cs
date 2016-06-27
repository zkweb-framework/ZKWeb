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
		/// 运行测试
		/// </summary>
		private static void RunTests() {
			// 初始化程序
			Application.Initialize(Startup.GetWebsiteRootDirectory());
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
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseIISIntegration()
				.UseStartup<Startup>()
				.Build();
			host.Run();
		}
	}
}
