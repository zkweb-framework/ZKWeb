using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.UnitTest.Event {
	/// <summary>
	/// 额外的错误信息
	/// </summary>
	public class ErrorMessageInfo {
		/// <summary>
		/// 错误信息
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="message">错误信息</param>
		public ErrorMessageInfo(string message) {
			Message = message;
		}
	}
}
