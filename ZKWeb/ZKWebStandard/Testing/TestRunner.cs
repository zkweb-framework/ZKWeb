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
	/// 测试运行器
	/// 运行器对应一个指定的程序集
	/// 运行测试的流程
	/// - 在程序集中查找带有测试属性的类
	/// - 创建这个类的实例，每个类都会有一个单独的线程
	/// - 查找所有公开的函数，每个函数都作为一个测试
	/// - 执行这些函数并向事件处理器报告结果
	/// - 如果类继承了IDisposable，调用Dispose函数
	/// </summary>
	public class TestRunner {
		/// <summary>
		/// 需要测试的程序集
		/// </summary>
		public Assembly Assembly { get; private set; }
		/// <summary>
		/// 测试的事件处理器
		/// </summary>
		public IList<ITestEventHandler> EventHandlers { get; private set; }
		/// <summary>
		/// 获取当前线程中使用的测试运行器
		/// 如果不在测试中则返回null
		/// </summary>
		public static TestRunner CurrentRunner { get { return currentRunner.Value; } }
		private static ThreadLocal<TestRunner> currentRunner = new ThreadLocal<TestRunner>();

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="assembly">需要测试的程序集</param>
		/// <param name="eventHandlers">测试的事件处理器</param>
		public TestRunner(Assembly assembly, IList<ITestEventHandler> eventHandlers) {
			Assembly = assembly;
			EventHandlers = eventHandlers;
		}

		/// <summary>
		/// 触发指定事件
		/// 调用事件处理器处理
		/// </summary>
		/// <typeparam name="T">事件信息的类型</typeparam>
		/// <param name="getAction">事件通知函数</param>
		/// <param name="info">事件信息</param>
		public void TriggerEvent<T>(Func<ITestEventHandler, Action<T>> getAction, T info) {
			EventHandlers.ForEach(h => getAction(h)(info));
		}

		/// <summary>
		/// 写入额外的错误信息
		/// 调用事件处理器处理
		/// </summary>
		/// <param name="message">错误信息</param>
		public void WriteErrorMessage(string message) {
			TriggerEvent(h => h.OnErrorMessage, new ErrorMessageInfo(this, message));
		}

		/// <summary>
		/// 写入额外的除错信息
		/// 调用事件处理器处理
		/// </summary>
		/// <param name="message"></param>
		public void WriteDebugMessage(string message) {
			TriggerEvent(h => h.OnDebugMessage, new DebugMessageInfo(this, message));
		}

		/// <summary>
		/// 运行测试函数
		/// </summary>
		/// <param name="method">测试函数</param>
		/// <param name="counter">测试结果的计数器</param>
		public void RunMethod(MethodInfo method, TestResultCounter counter) {
			// 每个测试函数都在独立线程中运行
			var type = method.DeclaringType;
			var thread = new Thread(() => {
				// 设置当前线程的运行器
				currentRunner.Value = this;
				// 重载Http上下文
				using (HttpManager.OverrideContext("", "GET")) {
					// 创建实例
					object instance = null;
					try {
						instance = Activator.CreateInstance(type);
					} catch (Exception ex) {
						WriteErrorMessage($"create instance of {type.Name} failed: {ex}");
						return;
					}
					// 调用测试函数
					try {
						TriggerEvent(h => h.OnTestStarting, new TestStartingInfo(this, method, instance));
						method.FastInvoke(instance);
						throw new AssertPassedException();
					} catch (AssertPassedException) {
						// 测试通过
						++counter.Passed;
						TriggerEvent(h => h.OnTestPassed, new TestPassedInfo(this, method, instance));
					} catch (AssertSkipedException ex) {
						// 测试跳过
						++counter.Skipped;
						TriggerEvent(h => h.OnTestSkipped, new TestSkippedInfo(this, method, instance, ex));
					} catch (Exception ex) {
						// 测试失败
						++counter.Failed;
						TriggerEvent(h => h.OnTestFailed, new TestFailedInfo(this, method, instance, ex));
					}
					// 如果测试类继承了IDisposable，则释放类
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
		/// 运行程序集中的所有测试函数
		/// </summary>
		public void Run() {
			// 测试前的处理
			TriggerEvent(h => h.OnAllTestStarting, new AllTestStartingInfo(this));
			// 创建计数器
			var counter = new TestResultCounter();
			// 枚举所有类型
			foreach (var type in Assembly.GetTypes()) {
				// 只测试带测试属性的类型
				if (type.GetTypeInfo().GetCustomAttribute<TestsAttribute>() == null) {
					continue;
				}
				// 执行测试函数
				foreach (var method in type.FastGetMethods(
					BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
					RunMethod(method, counter);
				}
			}
			// 测试后的处理
			TriggerEvent(h => h.OnAllTestCompleted, new AllTestCompletedInfo(this, counter));
		}
	}
}
