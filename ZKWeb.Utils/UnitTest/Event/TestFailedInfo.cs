using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.UnitTest.Event {
	/// <summary>
	/// 单项测试失败时的信息
	/// </summary>
	public class TestFailedInfo {
		/// <summary>
		/// 单元测试运行器
		/// </summary>
		public UnitTestRunner Runner { get; private set; }
		/// <summary>
		/// 测试函数
		/// </summary>
		public MethodInfo Method { get; private set; }
		/// <summary>
		/// 测试类的实例
		/// </summary>
		public object Instance { get; private set; }
		/// <summary>
		/// 导致测试失败的例外信息
		/// </summary>
		public Exception Exception { get; private set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="runner">单元测试运行器</param>
		/// <param name="method">测试函数</param>
		/// <param name="instance">测试类的实例</param>
		/// <param name="exception">导致测试失败的例外信息</param>
		public TestFailedInfo(
			UnitTestRunner runner, MethodInfo method, object instance, Exception exception) {
			Runner = runner;
			Method = method;
			Instance = instance;
			Exception = exception;
		}
	}
}
