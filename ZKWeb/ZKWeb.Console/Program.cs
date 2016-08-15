namespace ZKWeb.Console {
	using System;
	using System.IO;
	using System.Reflection;
	using Testing;
	using Testing.TestEventHandlers;

	/// <summary>
	/// Console program
	/// Used for internal testing
	/// </summary>
	internal class Program {
		/// <summary>
		/// Get website root directory
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
		/// Program entry
		/// </summary>
		/// <param name="args"></param>
		private static void Main(string[] args) {
			// Initialize application
			Application.Initialize(GetWebsiteRootDirectory());
			// Run all tests
			//var unitTestManager = Application.Ioc.Resolve<TestManager>();
			//unitTestManager.RunAllAssemblyTest(new TestConsoleEventHandler());
			// Done
			Console.WriteLine("done");
			Console.ReadLine();
		}
	}
}
