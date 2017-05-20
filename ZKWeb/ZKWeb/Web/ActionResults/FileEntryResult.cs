using System;
using ZKWeb.Storage;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// Read contents from file entry and write to response<br/>
	/// 从文件中读取内容并写入到回应<br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	/// <example>
	/// <code language="cs">
	/// public ExampleController : IController {
	///		[Action("example")]
	///		public IActionResult Example() {
	///			var fileStorage = Application.Ioc.Resolve&lt;IFileStorage&gt;();
	///			var fileEntry = fileStorage.GetResourceFile("static", "1.txt");
	///			return new FileEntryResult(fileEntry);
	///		}
	///	}
	/// </code>
	/// </example>
	public class FileEntryResult : IActionResult {
		/// <summary>
		/// File path<br/>
		/// 文件路径<br/>
		/// </summary>
		public IFileEntry FileEntry { get; set; }
		/// <summary>
		/// Cached modify time received from client<br/>
		/// 客户端缓存的内容的修改时间<br/>
		/// </summary>
		public DateTime? IfModifiedSince { get; set; }
		/// <summary>
		/// Range request in bytes<br/>
		/// 客户端要求的内容范围<br/>
		/// </summary>
		public Pair<long?, long?> BytesRange { get; set; }
		/// <summary>
		/// Content Type<br/>
		/// The default will be detected from the file extension<br/>
		/// 内容类型<br/>
		/// 默认会从文件后缀名检测<br/>
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="fileEntry">File entry</param>
		public FileEntryResult(IFileEntry fileEntry) :
			this(fileEntry, null, Pair.Create<long?, long?>(null, null)) { }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="fileEntry">File entry</param>
		/// <param name="ifModifiedSince">Cached modify time received from client</param>
		public FileEntryResult(IFileEntry fileEntry, DateTime? ifModifiedSince) :
			this(fileEntry, ifModifiedSince, Pair.Create<long?, long?>(null, null)) { }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="fileEntry">File entry</param>
		/// <param name="bytesRange">Range request in bytes</param>
		public FileEntryResult(IFileEntry fileEntry, Pair<long?, long?> bytesRange) :
			this(fileEntry, null, bytesRange) { }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="fileEntry">File entry</param>
		/// <param name="ifModifiedSince">Cached modify time received from client</param>
		/// <param name="bytesRange">Range request in bytes</param>
		public FileEntryResult(
			IFileEntry fileEntry, DateTime? ifModifiedSince, Pair<long?, long?> bytesRange) {
			FileEntry = fileEntry;
			IfModifiedSince = ifModifiedSince;
			BytesRange = bytesRange;
			ContentType = null;
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
			var lastModified = FileEntry.LastWriteTimeUtc.Truncate();
			response.SetLastModified(lastModified);
			// If file not modified, return 304
			// Otherwise write file to http response
			var contentType = ContentType ??
				MimeUtils.GetMimeType(FileEntry.Filename);
			if (IfModifiedSince != null && IfModifiedSince == lastModified) {
				response.StatusCode = 304;
				response.ContentType = contentType;
			} else {
				using (var stream = FileEntry.OpenRead())
				using (var streamResult = new StreamResult(stream, contentType, BytesRange)) {
					streamResult.WriteResponse(response);
				}
			}
		}
	}
}
