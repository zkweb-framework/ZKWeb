using System;
using System.IO;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// Write contents from stream to response<br/>
	/// 从数据流读取内容并写入到回应<br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	/// <example>
	/// <code language="cs">
	/// public ExampleController : IController {
	///		[Action("example")]
	///		public IActionResult Example() {
	///			var stream = new MemoryStream(new byte[] { 1, 2, 3 });
	///			return new StreamResult(stream);
	///		}
	///	}
	/// </code>
	/// </example>
	public class StreamResult : IActionResult, IDisposable {
		/// <summary>
		/// Stream object, auto dispose after wrote to response<br/>
		/// 数据流对象, 自动销毁<br/>
		/// </summary>
		public Stream Stream { get; set; }
		/// <summary>
		/// Mime type<br/>
		/// MIME类型<br/>
		/// </summary>
		public string ContentType { get; set; }
		/// <summary>
		/// Range request in bytes<br/>
		/// 要求返回的内容范围<br/>
		/// </summary>
		public Pair<long?, long?> BytesRange { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="stream">The stream</param>
		public StreamResult(Stream stream) :
			this(stream, "application/octet-stream") { }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <param name="contentType">Mime type</param>
		public StreamResult(Stream stream, string contentType) :
			this(stream, contentType, Pair.Create<long?, long?>(null, null)) { }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <param name="bytesRange">Range request in bytes</param>
		public StreamResult(Stream stream, Pair<long?, long?> bytesRange) :
			this(stream, "application/octet-stream", bytesRange) { }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <param name="contentType">Mime type</param>
		/// <param name="bytesRange">Range request in bytes</param>
		public StreamResult(Stream stream, string contentType, Pair<long?, long?> bytesRange) {
			Stream = stream;
			ContentType = contentType;
			BytesRange = bytesRange;
		}

		/// <summary>
		/// Write contents from stream to http response<br/>
		/// 从数据流读取内容并写入到http回应<br/>
		/// </summary>
		/// <param name="response">Http response</param>
		public void WriteResponse(IHttpResponse response) {
			if (Stream.CanSeek &&
				(BytesRange.First != null || BytesRange.Second != null)) {
				// Respect range request
				Stream.Seek(0, SeekOrigin.End);
				var length = Stream.Position;
				if ((BytesRange.First.HasValue && BytesRange.First.Value >= length) ||
					(BytesRange.Second.HasValue && BytesRange.Second.Value >= length) ||
					(BytesRange.First > BytesRange.Second)) {
					// Out of range
					response.StatusCode = 200;
					return;
				}
				var realBegin = BytesRange.First ?? 0;
				var realFinish = BytesRange.Second ?? (length - 1);
				response.StatusCode = 206;
				response.ContentType = ContentType;
				response.AddHeader("Accept-Ranges", "bytes");
				response.AddHeader("Content-Range", $"{realBegin}-{realFinish}/{length}");
				Stream.CopyTo(response.Body, 1024, realBegin, realFinish - realBegin + 1);
			} else {
				// Copy all contents
				response.StatusCode = 200;
				response.ContentType = ContentType;
				Stream.CopyTo(response.Body);
			}
			response.Body.Flush();
		}

		/// <summary>
		/// Dispose stream<br/>
		/// 释放数据流<br/>
		/// </summary>
		public void Dispose() {
			Stream.Dispose();
		}
	}
}
