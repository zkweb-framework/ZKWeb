using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.UnitTest {
	/// <summary>
	/// 断言失败抛出的例外
	/// </summary>
	public class AssertException : Exception {
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="reason">失败原因</param>
		public AssertException(string reason) : base(reason) { }
	}
}
