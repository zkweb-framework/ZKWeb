using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKWeb.Model {
	/// <summary>
	/// 请求结果的接口
	/// </summary>
	public interface IActionResult {
		/// <summary>
		/// 把数据写到http回应
		/// </summary>
		/// <param name="response">http回应</param>
		void WriteResponse(HttpResponse response);
	}
}
