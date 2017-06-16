namespace ${ProjectName}.Console {
	using System;
	using System.IO;
	using System.Reflection;
	using ZKWeb;
	using ZKWeb.Testing;
	using ZKWeb.Testing.TestEventHandlers;

	public class Program {
		public static void Main(string[] args) {
			RunTests();
		}

		public static void RunTests() {
			Application.Initialize(
				Path.Combine(Path.GetDirectoryName(typeof(Program).GetTypeInfo().Assembly.Location),
				"../../../../${ProjectName}.AspNetCore"));

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
