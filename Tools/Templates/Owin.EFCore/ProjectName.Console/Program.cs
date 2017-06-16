namespace ${ProjectName}.Console {
	using global::${ProjectName}.Owin;
	using Microsoft.Owin.Hosting;
	using System;
	using System.IO;
	using ZKWeb;
	using ZKWeb.Testing;
	using ZKWeb.Testing.TestEventHandlers;

	public class Program {
		public static void Main(string[] args) {
			// SelfHost();
			RunTests();
		}

		public static void SelfHost() {
			var url = "http://localhost:${SelfHostPort}";
			using (WebApp.Start<SelfHostStartup>(url)) {
				Console.WriteLine(string.Format("Open {0} in the browser or", url));
				Console.WriteLine("Press any key to continue..");
				Console.ReadLine();
			}
		}

		public static void RunTests() {
			Application.Initialize(new SelfHostStartup().GetWebsiteRootDirectory());

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

	public class SelfHostStartup : Startup {
		public override string GetWebsiteRootDirectory() {
			return Path.Combine(
				Path.GetDirectoryName(typeof(Program).Assembly.Location), "../../../${ProjectName}.Owin");
		}
	}
}
