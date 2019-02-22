namespace __ProjectName__.Console {
	using System;
	using System.IO;
	using ZKWeb;
	using ZKWeb.Testing;
	using ZKWeb.Testing.TestEventHandlers;

	public static class Program {
		public static void Main(string[] args) {
			RunTests();
		}

		public static void RunTests() {
			Application.Initialize(
				Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "../../../__ProjectName__.AspNet"));

			var testManager = Application.Ioc.Resolve<TestManager>();
			var testEventHandler = new TestConsoleEventHandler();
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
