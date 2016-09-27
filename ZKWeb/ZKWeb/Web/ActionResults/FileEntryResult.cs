using System;
using ZKWeb.Storage;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// Read contents from file entry and write to response
	/// </summary>
	public class FileEntryResult : IActionResult {
		/// <summary>
		/// File path
		/// </summary>
		public IFileEntry FileEntry { get; set; }
		/// <summary>
		/// Cached modify time received from client
		/// </summary>
		public DateTime? IfModifiedSince { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="fileEntry">File entry</param>
		/// <param name="ifModifiedSince">Cached modify time received from client</param>
		public FileEntryResult(IFileEntry fileEntry, DateTime? ifModifiedSince = null) {
			FileEntry = fileEntry;
			IfModifiedSince = ifModifiedSince;
		}

		/// <summary>
		/// Write file to http response
		/// If file not modified, return 304
		/// </summary>
		/// <param name="response">Http Reponse</param>
		public void WriteResponse(IHttpResponse response) {
			// Set last modified time
			var lastModified = FileEntry.LastWriteTimeUtc.Truncate();
			response.SetLastModified(lastModified);
			// Set mime
			response.ContentType = MimeUtils.GetMimeType(FileEntry.Filename);
			// If file not modified, return 304
			if (IfModifiedSince != null && IfModifiedSince == lastModified) {
				response.StatusCode = 304;
				return;
			}
			// Write file to http response
			response.StatusCode = 200;
			using (var stream = FileEntry.OpenRead()) {
				stream.CopyTo(response.Body);
				response.Body.Flush();
			}
		}
	}
}
