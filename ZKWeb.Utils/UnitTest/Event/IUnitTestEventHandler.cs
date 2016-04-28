using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ZKWeb.Utils.UnitTest.Event {
	/// <summary>
	/// 单元测试的事件处理器
	/// </summary>
	public interface IUnitTestEventHandler {
		/// <summary>
		/// 运行所有测试前的处理
		/// </summary>
		/// <param name="info">信息对象</param>
		void OnAllTestStarting(AllTestStartingInfo info);

		/// <summary>
		/// 所有测试运行完毕后的处理
		/// </summary>
		/// <param name="info">信息对象</param>
		void OnAllTestCompleted(AllTestCompletedInfo info);

		/// <summary>
		/// 单项测试开始时的处理
		/// </summary>
		/// <param name="info">信息对象</param>
		void OnTestStarting(TestStartingInfo info);

		/// <summary>
		/// 单项测试通过时的处理
		/// </summary>
		/// <param name="info">信息对象</param>
		void OnTestPassed(TestPassedInfo info);

		/// <summary>
		/// 单项测试失败时的处理
		/// </summary>
		/// <param name="info">信息对象</param>
		void OnTestFailed(TestFailedInfo info);

		/// <summary>
		/// 单项测试跳过时的处理
		/// </summary>
		/// <param name="info">信息对象</param>
		void OnTestSkipped(TestSkippedInfo info);

		/// <summary>
		/// 处理额外的错误信息
		/// 初始化或释放测试类的实例失败时会调用这个函数
		/// </summary>
		/// <param name="info">信息对象</param>
		void OnErrorMessage(ErrorMessageInfo info);

		/// <summary>
		/// 处理额外的除错信息
		/// 测试中需要输出除错信息时会调用这个函数
		/// </summary>
		/// <param name="info">信息对象</param>
		void OnDebugMessage(DebugMessageInfo info);
	}
}
