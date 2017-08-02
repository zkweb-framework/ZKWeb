namespace ZKWeb.Console {
	using System;
	using System.IO;
	using Testing;
	using Testing.TestEventHandlers;

	/// <summary>
	/// Console program<br/>
	/// Used for internal testing<br/>
	/// 控制台程序<br/>
	/// 用于内部测试<br/>
	/// </summary>
	internal class Program {
		/// <summary>
		/// Get website root directory<br/>
		/// 获取网站的根目录<br/>
		/// </summary>
		/// <returns></returns>
		private static string GetWebsiteRootDirectory() {
			var path = Path.GetDirectoryName(typeof(Program).Assembly.Location);
			while (!Directory.Exists(Path.Combine(path, "App_Data"))) {
				path = Path.GetDirectoryName(path);
				if (string.IsNullOrEmpty(path)) {
					throw new DirectoryNotFoundException("Website root directory not found");
				}
			}
			return path;
		}

		/// <summary>
		/// Program entry<br/>
		/// 程序入口点<br/>
		/// </summary>
		/// <param name="args"></param>
		private static void Main(string[] args) {
			// Initialize application
			Application.Initialize(GetWebsiteRootDirectory());
			// Run all tests
			var testManager = Application.Ioc.Resolve<TestManager>();
			var testEventHandler = new TestConsoleEventHandler();
			testManager.RunAllAssemblyTest(testEventHandler);
			if (testEventHandler.CompletedInfo.Counter.Failed > 0) {
				throw new Exception("Some test failed");
			} else {
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("All tests passed");
				Console.ResetColor();
			}
		}
	}
}
