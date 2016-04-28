using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.UnitTest.Event {
	/// <summary>
	/// 额外的除错信息
	/// </summary>
	public class DebugMessageInfo {
		/// <summary>
		/// 除错信息
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="message">除错信息</param>
		public DebugMessageInfo(string message) {
			Message = message;
		}
	}
}
