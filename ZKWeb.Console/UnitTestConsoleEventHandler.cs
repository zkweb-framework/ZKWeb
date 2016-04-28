namespace ZKWeb.Console {
	using System;
	using Utils.Extensions;
	using Utils.UnitTest.Event;

	/// <summary>
	/// 在控制台中运行单元测试使用的事件处理器
	/// </summary>
	public class UnitTestConsoleEventHandler : IUnitTestEventHandler {
		public void OnAllTestStarting(AllTestStartingInfo info) {
			Console.WriteLine($"starting {info.Runner.Assembly.GetName().Name} tests...");
		}

		public void OnAllTestCompleted(AllTestCompletedInfo info) {
			Console.WriteLine($"complete {info.Runner.Assembly.GetName().Name} tests: " +
				$"{info.Passed} passed, {info.Failed} failed, {info.Skiped} skiped.");
			Console.WriteLine();
		}

		public void OnDebugMessage(DebugMessageInfo info) {
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine($"debug: {info.Message}");
			Console.ResetColor();
		}

		public void OnErrorMessage(ErrorMessageInfo info) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"error: {info.Message}");
			Console.ResetColor();
		}

		public void OnTestFailed(TestFailedInfo info) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"failed: {info.Exception}");
			Console.ResetColor();
		}

		public void OnTestPassed(TestPassedInfo info) {
		}

		public void OnTestSkipped(TestSkippedInfo info) {
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"skipped: {info.Exception.Message}");
			Console.ResetColor();
		}

		public void OnTestStarting(TestStartingInfo info) {
			Console.WriteLine($"test: {info.Method.GetFullName()}");
		}
	}
}
