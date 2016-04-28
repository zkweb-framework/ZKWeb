using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.UnitTest.Event;

namespace ZKWeb.Utils.UnitTest {
	/// <summary>
	/// 测试运行器
	/// 运行器对应一个指定的程序集
	/// 运行测试的流程
	/// - 在程序集中查找带有UnitTestAttribute属性的类
	/// - 创建这个类的实例，每个类都会有一个单独的线程
	/// - 查找所有公开的函数，每个函数都作为一个测试
	/// - 执行这些函数并向事件处理器报告结果
	/// - 如果类继承了IDisposable，调用Dispose函数
	/// </summary>
	public class UnitTestRunner {
		/// <summary>
		/// 需要测试的程序集
		/// </summary>
		public Assembly Assembly { get; private set; }
		/// <summary>
		/// 单元测试的事件处理器
		/// </summary>
		public IList<IUnitTestEventHandler> EventHandlers { get; private set; }
		/// <summary>
		/// 获取当前线程中使用的单元测试运行器
		/// 如果不在测试中则返回null
		/// </summary>
		public static UnitTestRunner CurrentRunner { get { return _currentRunner.Value; } }
		private static ThreadLocal<UnitTestRunner> _currentRunner = new ThreadLocal<UnitTestRunner>();

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="assembly">需要测试的程序集</param>
		/// <param name="eventHandlers">单元测试的事件处理器</param>
		public UnitTestRunner(Assembly assembly, IList<IUnitTestEventHandler> eventHandlers) {
			Assembly = assembly;
			EventHandlers = eventHandlers;
		}

		/// <summary>
		/// 触发指定事件
		/// 调用事件处理器处理
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="getAction"></param>
		/// <param name="info"></param>
		public void TriggerEvent<T>(Func<IUnitTestEventHandler, Action<T>> getAction, T info) {
			EventHandlers.ForEach(h => getAction(h)(info));
		}

		/// <summary>
		/// 写入额外的错误信息
		/// 调用事件处理器处理
		/// </summary>
		/// <param name="message">错误信息</param>
		public void WriteErrorMessage(string message) {
			TriggerEvent(h => h.OnErrorMessage, new ErrorMessageInfo(message));
		}

		/// <summary>
		/// 写入额外的除错信息
		/// 调用事件处理器处理
		/// </summary>
		/// <param name="message"></param>
		public void WriteDebugMessage(string message) {
			TriggerEvent(h => h.OnDebugMessage, new DebugMessageInfo(message));
		}

		/// <summary>
		/// 运行测试
		/// </summary>
		public void Run() {
			// 测试前的处理
			TriggerEvent(h => h.OnAllTestStarting, new AllTestStartingInfo(this));
			// 计数器
			ulong passed = 0;
			ulong failed = 0;
			ulong skiped = 0;
			// 枚举所有类型
			foreach (var type in Assembly.GetTypes()) {
				// 只测试带UnitTestAttribute属性的类型
				if (type.GetCustomAttribute<UnitTestAttribute>() == null) {
					continue;
				}
				// 每个测试类都在新的线程中执行
				var thread = new Thread(() => {
					// 设置当前线程的运行器
					_currentRunner.Value = this;
					// 创建测试类
					object instance = null;
					try {
						instance = Activator.CreateInstance(type);
					} catch (Exception ex) {
						WriteErrorMessage($"create instance of {type.Name} failed: {ex}");
						return;
					}
					// 调用所有公开且在类中定义的函数
					foreach (var method in type.GetMethods(
						BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
						try {
							TriggerEvent(h => h.OnTestStarting, new TestStartingInfo(method, instance));
							((Action)Delegate.CreateDelegate(typeof(Action), instance, method))();
							throw new AssertPassedException();
						} catch (AssertPassedException) {
							// 测试通过
							passed += 1;
							TriggerEvent(h => h.OnTestPassed, new TestPassedInfo(method, instance));
						} catch (AssertSkipedException ex) {
							// 测试跳过
							skiped += 1;
							TriggerEvent(h => h.OnTestSkipped, new TestSkippedInfo(method, instance, ex));
						} catch (Exception ex) {
							// 测试失败
							failed += 1;
							TriggerEvent(h => h.OnTestFailed, new TestFailedInfo(method, instance, ex));
						}
					}
					// 如果测试类继承了IDisposable，则释放类
					try {
						(instance as IDisposable)?.Dispose();
					} catch (Exception ex) {
						WriteErrorMessage($"dispose instance of {type.Name} failed: {ex}");
						return;
					}
				});
				thread.Start();
				thread.Join();
			}
			// 测试后的处理
			TriggerEvent(h => h.OnAllTestCompleted, new AllTestCompletedInfo(this, passed, failed, skiped));
		}
	}
}
