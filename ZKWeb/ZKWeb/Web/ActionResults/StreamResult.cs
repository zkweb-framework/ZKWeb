using System;
using System.IO;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// Stream result
	/// </summary>
	public class StreamResult : IActionResult, IDisposable {
		/// <summary>
		/// Stream object, auto dispose after wrote to response
		/// </summary>
		public Stream Stream { get; set; }
		/// <summary>
		/// Mime type
		/// </summary>
		public string ContentType { get; set; }
		/// <summary>
		/// Range request in bytes
		/// </summary>
		public Pair<long?, long?> BytesRange { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="stream">The stream</param>
		public StreamResult(Stream stream) :
			this(stream, "application/octet-stream") { }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <param name="contentType">Mime type</param>
		public StreamResult(Stream stream, string contentType) :
			this(stream, contentType, Pair.Create<long?, long?>(null, null)) { }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <param name="bytesRange">Range request in bytes</param>
		public StreamResult(Stream stream, Pair<long?, long?> bytesRange) :
			this(stream, "application/octet-stream", bytesRange) { }

		/// <summary>
		/// Initialize
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
		/// Write stream to http response
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
		/// Dispose stream
		/// </summary>
		public void Dispose() {
			Stream.Dispose();
		}
	}
}
