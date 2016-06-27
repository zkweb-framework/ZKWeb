namespace ZKWeb.Console {
	using System;
	using System.IO;
	using Testing;
	using Testing.TestEventHandlers;

	class Program {
		static void Main(string[] args) {
			// 初始化程序
			Application.Initialize(
				Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "../../../../"));
			// 运行所有测试
			var unitTestManager = Application.Ioc.Resolve<TestManager>();
			unitTestManager.RunAllAssemblyTest(new TestConsoleEventHandler());
			// 等待结束
			Console.WriteLine("done");
			Console.ReadLine();
		}
	}
}
