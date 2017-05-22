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
	/// <br/>
	/// <br/>
	/// <br/>
	/// <br/>
	/// <br/>
	/// <br/>
	/// <br/>
	/// <br/>
	/// </summary>
	public class TestRunner {
		/// <summary>
		/// Test assembly
		/// </summary>
		public Assembly Assembly { get; private set; }
		/// <summary>
		/// Test event handlers
		/// </summary>
		public IList<ITestEventHandler> EventHandlers { get; private set; }
		/// <summary>
		/// The running test runner
		/// It should be null if no test runner is running
		/// </summary>
		public static TestRunner CurrentRunner { get { return currentRunner.Value; } }
		private static ThreadLocal<TestRunner> currentRunner = new ThreadLocal<TestRunner>();

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="assembly">Test assembly</param>
		/// <param name="eventHandlers">Test event handlers</param>
		public TestRunner(Assembly assembly, IList<ITestEventHandler> eventHandlers) {
			Assembly = assembly;
			EventHandlers = eventHandlers;
		}

		/// <summary>
		/// Triggering test event
		/// Will notify event handlers
		/// </summary>
		/// <typeparam name="T">Information type</typeparam>
		/// <param name="getAction">Event handle method</param>
		/// <param name="info">Information</param>
		public void TriggerEvent<T>(Func<ITestEventHandler, Action<T>> getAction, T info) {
			EventHandlers.ForEach(h => getAction(h)(info));
		}

		/// <summary>
		/// Write error message
		/// Will notify event handlers
		/// </summary>
		/// <param name="message">Error message</param>
		public void WriteErrorMessage(string message) {
			TriggerEvent(h => h.OnErrorMessage, new ErrorMessageInfo(this, message));
		}

		/// <summary>
		/// Write debug message
		/// Will notify event handlers
		/// </summary>
		/// <param name="message">Debug message</param>
		public void WriteDebugMessage(string message) {
			TriggerEvent(h => h.OnDebugMessage, new DebugMessageInfo(this, message));
		}

		/// <summary>
		/// Run test method
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
		/// Run all test cases in assembly
		/// </summary>
		public void Run() {
			// Triggering starting event
			TriggerEvent(h => h.OnAllTestStarting, new AllTestStartingInfo(this));
			// Create result counter
			var counter = new TestResultCounter();
			// Find all types in assembly have TestsAttribute 
			foreach (var type in Assembly.GetTypes()) {
				if (type.GetTypeInfo().GetCustomAttribute<TestsAttribute>() == null) {
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
