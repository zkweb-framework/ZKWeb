using System;
using System.IO;
using ZKWebStandard.Extensions;
using ZKWebStandard.Web;
using ZKWebStandard.Utils;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// File result
	/// </summary>
	[Obsolete("Read file directly from this method is not recommended")]
	public class FileResult : IActionResult {
		/// <summary>
		/// File path
		/// </summary>
		public string FilePath { get; set; }
		/// <summary>
		/// Cached modify time received from client
		/// </summary>
		public DateTime? IfModifiedSince { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="path">File path</param>
		/// <param name="ifModifiedSince">Cached modify time received from client</param>
		public FileResult(string path, DateTime? ifModifiedSince = null) {
			FilePath = path;
			IfModifiedSince = ifModifiedSince;
		}

		/// <summary>
		/// Write file to http response
		/// If file not modified, return 304
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
