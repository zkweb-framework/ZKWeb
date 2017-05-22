using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using ZKWebStandard.Web;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Http response extension methods<br/>
	/// <br/>
	/// </summary>
	public static class IHttpResponseExtensions {
		/// <summary>
		/// Redirect to url by javascript<br/>
		/// Use it instead of 301 redirect can make browser send the referer header<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="response">Http response</param>
		/// <param name="url">Url</param>
		public static void RedirectByScript(this IHttpResponse response, string url) {
			var urlJson = JsonConvert.SerializeObject(url);
			response.ContentType = "text/html";
			response.Write($@"<script type='text/javascript'>location.href = {urlJson};</script>");
			response.Body.Flush();
			response.End();
		}

		/// <summary>
		/// Set last modified time to http response<br/>
		/// <br/>
		/// </summary>
		/// <param name="response">Http response</param>
		/// <param name="date">Last modified time</param>
		public static void SetLastModified(this IHttpResponse response, DateTime date) {
			var value = date.ToUniversalTime().ToString("R", DateTimeFormatInfo.InvariantInfo);
			response.AddHeader("Last-Modified", value);
		}

		/// <summary>
		/// Write string to http response<br/>
		/// <br/>
		/// </summary>
		/// <param name="response">Http response</param>
		/// <param name="value">String value</param>
		public static void Write(this IHttpResponse response, string value) {
			var writer = new StreamWriter(response.Body);
			writer.Write(value);
			writer.Flush();
		}

		/// <summary>
		/// Write file to http response<br/>
		/// <br/>
		/// </summary>
		/// <param name="response">Http response</param>
		/// <param name="path">File path</param>
		public static void WriteFile(this IHttpResponse response, string path) {
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				stream.CopyTo(response.Body);
			}
			response.Body.Flush();
		}
	}
}
