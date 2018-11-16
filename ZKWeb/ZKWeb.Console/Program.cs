namespace ZKWeb.Console {
	using System;
	using System.IO;
	using System.Reflection;
	using Testing;
	using Testing.TestEventHandlers;

	/// <summary>
	/// Console program<br/>
	/// Used for internal testing<br/>
	/// 控制台程序<br/>
	/// 用于内部测试<br/>
	/// </summary>
	internal static class Program {
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
			Application.Initialize(GetWebsiteRootDirectory());

			var testManager = Application.Ioc.Resolve<TestManager>();
			var testEventHandler = new TestConsoleEventHandler();
			testManager.ExtraTestAssemblies.Add(Assembly.Load("Tests.ZKWeb"));
			testManager.ExtraTestAssemblies.Add(Assembly.Load("Tests.ZKWebStandard"));
			testManager.RunAllAssemblyTest(testEventHandler);

			var hasFailedCases = testEventHandler.CompletedInfo.Counter.Failed > 0;
			Console.ForegroundColor = hasFailedCases ? ConsoleColor.Red : ConsoleColor.Green;
			Console.WriteLine(string.Format(
				"complete all tests: {0} passed, {1} failed, {2} skipped",
				testEventHandler.CompletedInfo.Counter.Passed,
				testEventHandler.CompletedInfo.Counter.Failed,
				testEventHandler.CompletedInfo.Counter.Skipped));
			Console.ResetColor();
			Environment.Exit(hasFailedCases ? 1 : 0);
		}
	}
}