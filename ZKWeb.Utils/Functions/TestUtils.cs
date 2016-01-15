using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace ZKWeb.Utils.Functions {
	/// <summary>
	/// 测试工具类
	/// </summary>
	public static class TestUtils {
		/// <summary>
		/// 测试条件是否成立
		/// </summary>
		public static void Assert(bool condition,
			[CallerMemberName] string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0) {
			if (!condition) {
				throw new TestException($"Assert failed: {filePath}:{lineNumber} {memberName}");
			}
		}

		/// <summary>
		/// 测试函数执行是否出错
		/// </summary>
		public static void Assert(Action action,
			[CallerMemberName] string memberName = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0) {
			try {
				action();
			} catch (Exception ex) {
				throw new TestException($"Assert failed: {filePath}:{lineNumber} {memberName}, ex: {ex.ToString()}");
			}
		}

		/// <summary>
		/// 运行当前程序域中所有程序集的测试
		/// </summary>
		public static void RunAllTests() {
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				if (assembly.GlobalAssemblyCache) {
					continue; // 跳过系统程序集
				}
				RunAssemblyTest(assembly);
			}
		}

		/// <summary>
		/// 运行指定程序集的测试
		/// </summary>
		public static void RunAssemblyTest(string assemblyName) {
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				if (assembly.GetName().Name == assemblyName) {
					RunAssemblyTest(assembly);
					return;
				}
			}
			throw new KeyNotFoundException($"assembly {assemblyName} not found");
		}

		/// <summary>
		/// 运行指定程序集的测试
		/// </summary>
		public static void RunAssemblyTest(Assembly assembly) {
			foreach (var type in assembly.GetTypes()) {
				// 只测试带TestCollection属性的类型
				if (type.GetCustomAttribute<TestCollectionAttribute>() == null) {
					continue;
				}
				// 每个测试类都在新的线程中执行
				Console.WriteLine($"Run Test {type.FullName}");
				var thread = new Thread(() => {
					try {
						IsTestMode.Value = true; // 设置测试模式
						var instance = Activator.CreateInstance(type);
						foreach (var method in type.GetMethods(
							BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
							method.Invoke(instance, null); // 调用所有公开且在类中定义的函数
						}
					} catch (Exception ex) {
						if (ex.InnerException != null) {
							ex = ex.InnerException; // 从InvokeException中取出实际的例外
						};
						Console.WriteLine(ex.ToString());
						Console.Write("Continue? (y/N) ");
						if (Console.ReadLine().Trim().ToLower() != "y") {
							Environment.Exit(0);
						}
					}
				});
				thread.Start();
				thread.Join();
			}
		}

		/// <summary>
		/// 是否测试模式
		/// </summary>
		/// <returns></returns>
		public static ThreadLocal<bool> IsTestMode { get; set; } = new ThreadLocal<bool>();
	}

	/// <summary>
	/// 测试失败时抛出的例外
	/// </summary>
	public class TestException : Exception {
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="reason"></param>
		public TestException(string reason) : base(reason) { }
	}

	/// <summary>
	/// 测试集合的属性
	/// 用于标记一个类为测试集合
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class TestCollectionAttribute : Attribute {
	}
}
