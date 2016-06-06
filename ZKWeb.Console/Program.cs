namespace ZKWeb.Console {
	using DryIoc;
	using Localize;
	using System;
	using UnitTest;

	public class A {

	}

	class Program {
		static void Main(string[] args) {
			new Application().Application_Start();

			// var unitTestManager = Application.Ioc.Resolve<UnitTestManager>();
			// unitTestManager.RunAllAssemblyTest(new UnitTestConsoleEventHandler());
			using (Application.OverrideIoc()) {
				var translateManager = Application.Ioc.Resolve<TranslateManager>();
				Application.Ioc.RegisterMany<A>();
				using (Application.OverrideIoc()) {
					Console.WriteLine(Application.Ioc.Resolve<A>());
				}
			}
			Console.WriteLine(Application.Ioc.Resolve<A>(IfUnresolved.ReturnDefault));


			Console.WriteLine("done");
			Console.ReadLine();
		}
	}
}
