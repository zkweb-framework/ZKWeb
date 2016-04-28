using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.UnitTest.Event {
	/// <summary>
	/// 所有测试运行完毕后的信息
	/// </summary>
	public class AllTestCompletedInfo {
		/// <summary>
		/// 单元测试运行器
		/// </summary>
		public UnitTestRunner Runner { get; private set; }
		/// <summary>
		/// 成功的测试数量
		/// </summary>
		public ulong Passed { get; private set; }
		/// <summary>
		/// 失败的测试数量
		/// </summary>
		public ulong Failed { get; private set; }
		/// <summary>
		/// 跳过的测试数量
		/// </summary>
		public ulong Skiped { get; private set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="runner">单元测试运行器</param>
		/// <param name="passed">成功的测试数量</param>
		/// <param name="failed">失败的测试数量</param>
		/// <param name="skiped">跳过的测试数量</param>
		public AllTestCompletedInfo(UnitTestRunner runner, ulong passed, ulong failed, ulong skiped) {
			Runner = runner;
			Passed = passed;
			Failed = failed;
			Skiped = skiped;
		}
	}
}
