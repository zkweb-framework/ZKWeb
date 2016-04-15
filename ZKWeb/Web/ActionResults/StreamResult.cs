using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Web.Interfaces;

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
		/// 写入到http回应中
		/// </summary>
		/// <param name="response">http回应</param>
		public void WriteResponse(HttpResponse response) {
			response.ContentType = ContentType;
			var buffer = new byte[1024];
			while (true) {
				var readBytes = Stream.Read(buffer, 0, 1024);
				if (readBytes <= 0) {
					break;
				}
				response.OutputStream.Write(buffer, 0, readBytes);
			}
		}

		/// <summary>
		/// 清理资源
		/// </summary>
		public void Dispose() {
			Stream.Dispose();
		}
	}
}
