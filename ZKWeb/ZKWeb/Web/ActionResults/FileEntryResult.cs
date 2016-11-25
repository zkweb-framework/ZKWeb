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
			// If file not modified, return 304
			// Otherwise write file to http response
			var contentType = MimeUtils.GetMimeType(FileEntry.Filename);
			if (IfModifiedSince != null && IfModifiedSince == lastModified) {
				response.StatusCode = 304;
				response.ContentType = contentType;
			} else {
				using (var stream = FileEntry.OpenRead())
				using (var streamResult = new StreamResult(stream, contentType)) {
					streamResult.WriteResponse(response);
				}
			}
		}
	}
}
