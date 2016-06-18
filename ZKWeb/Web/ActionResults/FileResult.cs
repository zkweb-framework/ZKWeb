using System;
using System.IO;
using ZKWebStandard.Extensions;
using ZKWeb.Web;
using ZKWebStandard.Web;
using ZKWebStandard.Utils;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// 文件结果
	/// </summary>
	public class FileResult : IActionResult {
		/// <summary>
		/// 文件路径
		/// </summary>
		public string FilePath { get; set; }
		/// <summary>
		/// 客户端传入的文件修改时间
		/// </summary>
		public DateTime? IfModifiedSince { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="path">文件路径</param>
		/// <param name="ifModifiedSince">客户端传入的文件修改时间</param>
		public FileResult(string path, DateTime? ifModifiedSince = null) {
			FilePath = path;
			IfModifiedSince = ifModifiedSince;
		}

		/// <summary>
		/// 写入文件到Http回应
		/// </summary>
		/// <param name="response">Http回应</param>
		public void WriteResponse(IHttpResponse response) {
			// 设置文件的最后修改时间
			var lastModified = File.GetLastWriteTimeUtc(FilePath).Truncate();
			response.SetLastModified(lastModified);
			// 设置内容类型
			response.ContentType = MimeUtils.GetMimeType(FilePath);
			// 文件没有修改时返回304
			if (IfModifiedSince != null && IfModifiedSince == lastModified) {
				response.StatusCode = 304;
				return;
			}
			// 写入文件到http回应中
			response.StatusCode = 200;
			response.WriteFile(FilePath);
		}
	}
}
