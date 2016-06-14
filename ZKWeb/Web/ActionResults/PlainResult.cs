using ZKWeb.Web.Abstractions;
using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

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
		/// 写入文本到Http回应
		/// </summary>
		/// <param name="response">Http回应</param>
		public void WriteResponse(IHttpResponse response) {
			// 设置状态代码和内容类型
			response.StatusCode = 200;
			response.ContentType = "text/plain";
			// 写入文本到Http回应
			response.Write(Text);
		}
	}
}
