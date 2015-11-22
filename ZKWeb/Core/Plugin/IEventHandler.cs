using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKWeb.Core.Plugin {
	/// <summary>
	/// 事件处理器的接口
	/// </summary>
	public interface IEventHandler {
		/// <summary>
		/// 处理事件的函数
		/// </summary>
		/// <param name="arg">参数</param>
		/// <param name="stop">是否停止之后的处理</param>
		void Handle(object arg, ref bool stop);
	}
}
