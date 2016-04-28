using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.UnitTest.Event {
	/// <summary>
	/// 单项测试通过时的信息
	/// </summary>
	public class TestPassedInfo {
		/// <summary>
		/// 测试函数
		/// </summary>
		public MethodInfo Method { get; private set; }
		/// <summary>
		/// 测试类的实例
		/// </summary>
		public object Instance { get; private set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="method">测试函数</param>
		/// <param name="instance">测试类的实例</param>
		public TestPassedInfo(MethodInfo method, object instance) {
			Method = method;
			Instance = instance;
		}
	}
}
