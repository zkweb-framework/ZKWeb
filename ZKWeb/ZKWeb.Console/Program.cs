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
			// Use specified temporary database
			/*var ticks = DateTime.UtcNow.Ticks;
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			var websiteConfig = configManager.WebsiteConfig;
			websiteConfig.Extra["ZKWeb.TemporaryDatabaseORM"] = "NHibernate";
			websiteConfig.Extra["ZKWeb.TemporaryDatabaseType"] = "SQLite";
			websiteConfig.Extra["ZKWeb.TemporaryDatabaseConnectionString"] =
				$"Data Source={{{{App_Data}}}}/test_{ticks}.db;Version=3;";*/
			// Run all tests
			var unitTestManager = Application.Ioc.Resolve<TestManager>();
			unitTestManager.RunAllAssemblyTest(new TestConsoleEventHandler());
			// Done
			Console.WriteLine("done");
			Console.ReadLine();
		}
	}
}
