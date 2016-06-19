using System;
using System.IO;
using ZKWeb.Web;
using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// 数据流结果
	/// </summary>
	public class StreamResult : IActionResult, IDisposable {
		/// <summary>
		/// 数据流对象
		/// </summary>
		public Stream Stream { get; set; }
		/// <summary>
		/// 内容类型
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="stream">数据流对象</param>
		/// <param name="contentType">内容类型，默认是application/octet-stream</param>
		public StreamResult(Stream stream, string contentType = null) {
			Stream = stream;
			ContentType = contentType ?? "application/octet-stream";
		}

		/// <summary>
		/// 写入数据流到Http回应
		/// </summary>
		/// <param name="response">http回应</param>
		public void WriteResponse(IHttpResponse response) {
			// 设置状态代码和内容类型
			response.StatusCode = 200;
			response.ContentType = ContentType;
			// 写入数据流到Http回应
			Stream.CopyTo(response.Body);
			response.Body.Flush();
		}

		/// <summary>
		/// 清理资源
		/// </summary>
		public void Dispose() {
			Stream.Dispose();
		}
	}
}
