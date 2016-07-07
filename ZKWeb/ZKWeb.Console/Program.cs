namespace ZKWeb.Console {
	using System;
	using System.IO;
	using System.Reflection;
	using Testing;
	using Testing.TestEventHandlers;

	/// <summary>
	/// 控制台程序
	/// 主要用来运行测试
	/// </summary>
	internal class Program {
		/// <summary>
		/// 获取网站根目录
		/// </summary>
		/// <returns></returns>
		private static string GetWebsiteRootDirectory() {
			var path = Path.GetDirectoryName(typeof(Program).GetTypeInfo().Assembly.Location);
			while (!Directory.Exists(Path.Combine(path, "App_Data"))) {
				path = Path.GetDirectoryName(path);
				if (string.IsNullOrEmpty(path)) {
					throw new DirectoryNotFoundException("Website root directory not found");
				}
			}
			return path;
		}

		/// <summary>
		/// 控制台程序入口点
		/// </summary>
		/// <param name="args"></param>
		private static void Main(string[] args) {
			// 初始化程序
			Application.Initialize(GetWebsiteRootDirectory());
			// 运行所有测试
			var unitTestManager = Application.Ioc.Resolve<TestManager>();
			unitTestManager.RunAllAssemblyTest(new TestConsoleEventHandler());
			// 等待结束
			Console.WriteLine("done");
			Console.ReadLine();
		}
	}
}
