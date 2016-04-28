namespace ZKWeb.Console {
	using DryIoc;
	using System;
	using UnitTest;

	class Program {
		static void Main(string[] args) {
			new Application().Application_Start();

			var unitTestManager = Application.Ioc.Resolve<UnitTestManager>();
			unitTestManager.RunAllAssemblyTest(new UnitTestConsoleEventHandler());

			Console.WriteLine("done");
			Console.ReadLine();
		}
	}
}
