using System;
using System.IO;
using ZKWebStandard.Extensions;
using ZKWebStandard.Web;
using ZKWebStandard.Utils;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// File result<br/>
	/// 文件结果, 此类已经过时, 请使用FileEntryResult<br/>
	/// </summary>
	[Obsolete("Please use FileEntryResult")]
	public class FileResult : IActionResult {
		/// <summary>
		/// File path<br/>
		/// 文件路径<br/>
		/// </summary>
		public string FilePath { get; set; }
		/// <summary>
		/// Cached modify time received from client<br/>
		/// 客户端缓存的内容的修改时间<br/>
		/// </summary>
		public DateTime? IfModifiedSince { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="path">File path</param>
		/// <param name="ifModifiedSince">Cached modify time received from client</param>
		public FileResult(string path, DateTime? ifModifiedSince = null) {
			FilePath = path;
			IfModifiedSince = ifModifiedSince;
		}

		/// <summary>
		/// Write file to http response<br/>
		/// Return 304 If the file is not modified after the client caches it<br/>
		/// 写入文件到http回应<br/>
		/// 如果文件与客户端缓存的内容一致则返回304<br/>
		/// </summary>
		/// <param name="response">Http Reponse</param>
		public void WriteResponse(IHttpResponse response) {
			// Set last modified time
			var lastModified = File.GetLastWriteTimeUtc(FilePath).Truncate();
			response.SetLastModified(lastModified);
			// Set mime
			response.ContentType = MimeUtils.GetMimeType(FilePath);
			// If file not modified, return 304
			if (IfModifiedSince != null && IfModifiedSince == lastModified) {
				response.StatusCode = 304;
				return;
			}
			// Write file to http response
			response.StatusCode = 200;
			response.WriteFile(FilePath);
		}
	}
}
