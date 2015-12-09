using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ZKWeb.Model.ActionResults {
	/// <summary>
	/// 文件结果
	/// </summary>
	public class FileResult : IActionResult {
		/// <summary>
		/// 文件路径
		/// </summary>
		public string FilePath { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="path">文件路径</param>
		public FileResult(string path) {
			FilePath = path;
		}

		/// <summary>
		/// 写入文件到http回应中
		/// </summary>
		/// <param name="response"></param>
		public void WriteResponse(HttpResponse response) {
			response.ContentType = MimeMapping.GetMimeMapping(FilePath);
			response.WriteFile(FilePath);
		}
	}
}
