using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZKWebStandard.Testing {
	/// <summary>
	/// 测试结果的计数器
	/// </summary>
	public class TestResultCounter {
		/// <summary>
		/// 成功的测试数量
		/// </summary>
		public ulong Passed { get; set; }
		/// <summary>
		/// 失败的测试数量
		/// </summary>
		public ulong Failed { get; set; }
		/// <summary>
		/// 跳过的测试数量
		/// </summary>
		public ulong Skipped { get; set; }
	}
}
