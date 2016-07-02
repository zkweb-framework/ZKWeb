namespace ${ProjectName}.Console {
	using System;
	using System.IO;
	using ZKWeb;
	using ZKWeb.Testing;
	using ZKWeb.Testing.TestEventHandlers;

	public class Program {
		public static void Main(string[] args) {
			RunTests();
		}

		public static void RunTests() {
			Application.Initialize(
				Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location),
				"../../../../../${ProjectName}"));

			var unitTestManager = Application.Ioc.Resolve<TestManager>();
			unitTestManager.RunAllAssemblyTest(new TestConsoleEventHandler());

			Console.WriteLine("done");
			Console.ReadLine();
		}
	}
}
