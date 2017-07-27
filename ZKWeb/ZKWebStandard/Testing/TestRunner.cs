using System;
using System.Collections.Generic;
using System.FastReflection;
using System.Reflection;
using System.Threading;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing.Events;
using ZKWebStandard.Web;

namespace ZKWebStandard.Testing {
	/// <summary>
	/// Test runner<br/>
	/// Test runner is bound to single assembly<br/>
	/// Testing flow<br/>
	/// - Find all types in assembly have TestsAttribute<br/>
	/// - Find all public methods in test class, every method is a test case and run in separate thread<br/>
	/// - Create test instance<br/>
	/// - Execute test methods and notify event handlers<br/>
	/// - If test instance is disposable, dispose it<br/>
	/// 测试运行器<br/>
	/// 单个测试运行器与单个程序集绑定<br/>
	/// 测试流程<br/>
	/// - 查找程序集中的类型，找出标记了TestsAttribute的类型<br/>
	/// - 查找类型中所有公开的函数，函数是单独的测试并且会运行在不同的线程中<br/>
	/// - 创建类型的实例<br/>
	/// - 执行测试函数并且通知事件处理器<br/>
	/// - 如果类型的实例有销毁函数(Dispose)，则调用销毁函数<br/>
	/// </summary>
	/// <seealso cref="Assert"/>
	/// <seealso cref="ITestEventHandler"/>
	/// <example>
	/// <code language="cs">
	/// [Tests]
	/// class ExampleTest {
	/// 	public void MethodA() {
	/// 		Assert.IsTrue(1 == 1);
	/// 		Assert.IsTrueWith(1 == 1, "if failed this item will be outputed");
	/// 		Assert.Equals(true, true);
	/// 		Assert.Throws&lt;ArgumentException&gt;(() =&gt; { throw new ArgumentException(); });
	/// 	}
	/// }
	///
	/// public class TestConsoleEventHandler : ITestEventHandler {
	/// 	public void OnAllTestStarting(AllTestStartingInfo info) {
	/// 		Console.WriteLine($"starting {info.Runner.Assembly.GetName().Name} tests...");
	/// 	}
	///
	/// 	public void OnAllTestCompleted(AllTestCompletedInfo info) {
	/// 		Console.WriteLine($"complete {info.Runner.Assembly.GetName().Name} tests: " +
	/// 			$"{info.Counter.Passed} passed, {info.Counter.Failed} failed, {info.Counter.Skipped} skiped.");
	/// 		Console.WriteLine();
	/// 	}
	///
	/// 	public void OnDebugMessage(DebugMessageInfo info) {
	/// 		Console.WriteLine($"debug: {info.Message}");
	/// 	}
	///
	/// 	public void OnErrorMessage(ErrorMessageInfo info) {
	/// 		Console.WriteLine($"error: {info.Message}");
	/// 	}
	///
	/// 	public void OnTestFailed(TestFailedInfo info) {
	/// 		Console.WriteLine($"failed: {info.Exception}");
	/// 	}
	///
	/// 	public void OnTestPassed(TestPassedInfo info) {
	/// 	}
	///
	/// 	public void OnTestSkipped(TestSkippedInfo info) {
	/// 		Console.WriteLine($"skipped: {info.Exception.Message}");
	/// 	}
	///
	/// 	public void OnTestStarting(TestStartingInfo info) {
	/// 		Console.WriteLine($"test: {info.Method.GetFullName()}");
	/// 	}
	/// }
	/// 
	/// var assembly = typeof(ExampleTest).Assembly;
	/// var handler = new TestConsoleEventHandler();
	/// var runner = new TestRunner(assembly, handler);
	/// runner.Run();
	/// </code>
	/// </example>
	public class TestRunner {
		/// <summary>
		/// Test assembly<br/>
		/// 测试的程序集<br/>
		/// </summary>
		public Assembly Assembly { get; private set; }
		/// <summary>
		/// Test event handlers<br/>
		/// 事件处理器列表<br/>
		/// </summary>
		public IList<ITestEventHandler> EventHandlers { get; private set; }
		/// <summary>
		/// The running test runner<br/>
		/// It should be null if no test runner is running<br/>
		/// 测试运行器的实例<br/>
		/// 如果当前无测试运行应该是null<br/>
		/// </summary>
		public static TestRunner CurrentRunner { get { return currentRunner.Value; } }
		private static ThreadLocal<TestRunner> currentRunner = new ThreadLocal<TestRunner>();

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="assembly">Test assembly</param>
		/// <param name="eventHandlers">Test event handlers</param>
		public TestRunner(Assembly assembly, IList<ITestEventHandler> eventHandlers) {
			Assembly = assembly;
			EventHandlers = eventHandlers;
		}

		/// <summary>
		/// Triggering test event<br/>
		/// Will notify event handlers<br/>
		/// 触发测试事件<br/>
		/// 会通知事件处理器<br/>
		/// </summary>
		/// <typeparam name="T">Information type</typeparam>
		/// <param name="getAction">Event handle method</param>
		/// <param name="info">Information</param>
		public void TriggerEvent<T>(Func<ITestEventHandler, Action<T>> getAction, T info) {
			EventHandlers.ForEach(h => getAction(h)(info));
		}

		/// <summary>
		/// Write error message<br/>
		/// Will notify event handlers<br/>
		/// 写入错误消息<br/>
		/// 会触发事件处理器<br/>
		/// </summary>
		/// <param name="message">Error message</param>
		public void WriteErrorMessage(string message) {
			TriggerEvent(h => h.OnErrorMessage, new ErrorMessageInfo(this, message));
		}

		/// <summary>
		/// Write debug message<br/>
		/// Will notify event handlers<br/>
		/// 写入除错消息<br/>
		/// 会触发事件处理器<br/>
		/// </summary>
		/// <param name="message">Debug message</param>
		public void WriteDebugMessage(string message) {
			TriggerEvent(h => h.OnDebugMessage, new DebugMessageInfo(this, message));
		}

		/// <summary>
		/// Run the test function in a separate thread<br/>
		/// 在独立的线程中运行测试函数<br/>
		/// </summary>
		/// <param name="method">Test method</param>
		/// <param name="counter">Test result counter</param>
		public void RunMethod(MethodInfo method, TestResultCounter counter) {
			// Run method in separate thread
			var type = method.DeclaringType;
			var thread = new Thread(() => {
				// Set running runner
				currentRunner.Value = this;
				// Override http context
				using (HttpManager.OverrideContext("", "GET")) {
					// Create test instance
					object instance = null;
					try {
						instance = Activator.CreateInstance(type);
					} catch (Exception ex) {
						WriteErrorMessage($"create instance of {type.Name} failed: {ex}");
						return;
					}
					// Call test method
					try {
						TriggerEvent(h => h.OnTestStarting, new TestStartingInfo(this, method, instance));
						method.FastInvoke(instance);
						throw new AssertPassedException();
					} catch (AssertPassedException) {
						// Test passed
						++counter.Passed;
						TriggerEvent(h => h.OnTestPassed, new TestPassedInfo(this, method, instance));
					} catch (AssertSkipedException ex) {
						// Test skipped
						++counter.Skipped;
						TriggerEvent(h => h.OnTestSkipped, new TestSkippedInfo(this, method, instance, ex));
					} catch (Exception ex) {
						// Test failed
						++counter.Failed;
						TriggerEvent(h => h.OnTestFailed, new TestFailedInfo(this, method, instance, ex));
					}
					// If test instance is disposable, dispose it
					try {
						(instance as IDisposable)?.Dispose();
					} catch (Exception ex) {
						WriteErrorMessage($"dispose instance of {type.Name} failed: {ex}");
						return;
					}
				}
			});
			thread.Start();
			thread.Join();
		}

		/// <summary>
		/// Run all tests in assembly<br/>
		/// 运行程序集中包含的所有测试<br/>
		/// </summary>
		public void Run() {
			// Triggering starting event
			TriggerEvent(h => h.OnAllTestStarting, new AllTestStartingInfo(this));
			// Create result counter
			var counter = new TestResultCounter();
			// Find all types in assembly have TestsAttribute 
			foreach (var type in Assembly.GetTypes()) {
				if (type.GetCustomAttribute<TestsAttribute>() == null) {
					continue;
				}
				// Find and run public methods
				foreach (var method in type.FastGetMethods(
					BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
					RunMethod(method, counter);
				}
			}
			// Triggering completed event
			TriggerEvent(h => h.OnAllTestCompleted, new AllTestCompletedInfo(this, counter));
		}
	}
}
