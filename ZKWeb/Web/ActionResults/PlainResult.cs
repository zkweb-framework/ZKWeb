using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ZKWeb.Web.Interfaces;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// 纯文本结果
	/// </summary>
	public class PlainResult : IActionResult {
		/// <summary>
		/// 写入的文本
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="obj"></param>
		public PlainResult(object obj) {
			Text = obj?.ToString();
		}

		/// <summary>
		/// 写入到http回应
		/// </summary>
		/// <param name="response">http回应</param>
		public void WriteResponse(HttpResponse response) {
			response.ContentType = "text/plain";
			response.Write(Text);
		}
	}
}
