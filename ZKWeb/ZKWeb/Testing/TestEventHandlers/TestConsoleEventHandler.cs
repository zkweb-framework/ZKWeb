using System;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing.Events;

namespace ZKWeb.Testing.TestEventHandlers {
	/// <summary>
	/// Test event handler for running tests from console<br/>
	/// 控制台运行测试使用的事件处理器<br/>
	/// </summary>
	/// <seealso cref="TestManager"/>
	public class TestConsoleEventHandler : ITestEventHandler {
#pragma warning disable CS1591
		public AllTestCompletedInfo CompletedInfo { get; set; }

		public void OnAllTestStarting(AllTestStartingInfo info) {
			Console.WriteLine($"starting {info.Runner.Assembly.GetName().Name} tests...");
		}

		public void OnAllTestCompleted(AllTestCompletedInfo info) {
			Console.ForegroundColor = (info.Counter.Failed > 0) ? ConsoleColor.Red : ConsoleColor.Green;
			Console.WriteLine($"complete {info.Runner.Assembly.GetName().Name} tests: " +
				$"{info.Counter.Passed} passed, {info.Counter.Failed} failed, {info.Counter.Skipped} skiped.");
			Console.WriteLine();
			Console.ResetColor();
			CompletedInfo = info;
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
#pragma warning restore CS1591
	}
}
