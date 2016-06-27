namespace ZKWeb.AspNet.Console {
	using System;
	using System.IO;
	using Testing;
	using Testing.TestEventHandlers;

	public class Program {
		public static void Main(string[] args) {
			RunTests();
		}

		public static void RunTests() {
			Application.Initialize(
				Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "../../../ZKWeb.AspNet"));

			var unitTestManager = Application.Ioc.Resolve<TestManager>();
			unitTestManager.RunAllAssemblyTest(new TestConsoleEventHandler());

			Console.WriteLine("done");
			Console.ReadLine();
		}
	}
}
