using System;
using System.IO;
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
		/// Initialize
		/// </summary>
		/// <param name="stream">The stream</param>
		/// <param name="contentType">Mime type, default is application/octet-stream</param>
		public StreamResult(Stream stream, string contentType = null) {
			Stream = stream;
			ContentType = contentType ?? "application/octet-stream";
		}

		/// <summary>
		/// Write stream to http response
		/// </summary>
		/// <param name="response">Http response</param>
		public void WriteResponse(IHttpResponse response) {
			// Set status and mime
			response.StatusCode = 200;
			response.ContentType = ContentType;
			// Write stream to http response
			// TODO: support range request
			// http://dotnetslackers.com/articles/aspnet/Range-Specific-Requests-in-ASP-NET.aspx
			// http://www.freesoft.org/CIE/RFC/2068/178.htm
			//
			// http://stackoverflow.com/questions/3303029/http-range-header
			Stream.CopyTo(response.Body);
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
