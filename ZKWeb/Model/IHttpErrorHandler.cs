using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Model {
	/// <summary>
	/// 网站程序错误的处理器接口
	/// </summary>
	public interface IHttpErrorHandler {
		/// <summary>
		/// 网站程序出错时的处理
		/// </summary>
		void OnError(Exception ex);
	}
}
