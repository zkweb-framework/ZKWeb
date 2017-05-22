using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using ZKWebStandard.Web;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Http response extension methods<br/>
	/// Http回应的扩展函数<br/>
	/// </summary>
	public static class IHttpResponseExtensions {
		/// <summary>
		/// Redirect to url by javascript<br/>
		/// Use it instead of 301 redirect can make browser send the referer header<br/>
		/// 使用javascript重定向到Url<br/>
		/// 用来代替301跳转可以让浏览器发送Referer头<br/>
		/// </summary>
		/// <param name="response">Http response</param>
		/// <param name="url">Url</param>
		/// <example>
		/// <code language="cs">
		/// HttpManager.CurrentContext.Response.RedirectByScript("/login");
		/// </code>
		/// </example>
		public static void RedirectByScript(this IHttpResponse response, string url) {
			var urlJson = JsonConvert.SerializeObject(url);
			response.ContentType = "text/html";
			response.Write($@"<script type='text/javascript'>location.href = {urlJson};</script>");
			response.Body.Flush();
			response.End();
		}

		/// <summary>
		/// Set last modified time to http response<br/>
		/// 设置内容的最后修改时间到Http回应<br/>
		/// </summary>
		/// <param name="response">Http response</param>
		/// <param name="date">Last modified time</param>
		/// <example>
		/// <code language="cs">
		/// HttpManager.CurrentContext.Response.SetLastModified(new DateTime(2016, 06, 13, 03, 09, 22, DateTimeKind.Utc));
		/// </code>
		/// </example>
		public static void SetLastModified(this IHttpResponse response, DateTime date) {
			var value = date.ToUniversalTime().ToString("R", DateTimeFormatInfo.InvariantInfo);
			response.AddHeader("Last-Modified", value);
		}

		/// <summary>
		/// Write string to http response<br/>
		/// 写入字符串到Http回应<br/>
		/// </summary>
		/// <param name="response">Http response</param>
		/// <param name="value">String value</param>
		/// <example>
		/// <code language="cs">
		/// HttpManager.CurrentContext.Response.Write("string contents");
		/// </code>
		/// </example>
		public static void Write(this IHttpResponse response, string value) {
			var writer = new StreamWriter(response.Body);
			writer.Write(value);
			writer.Flush();
		}

		/// <summary>
		/// Write file to http response<br/>
		/// 写入文件到Http回应<br/>
		/// </summary>
		/// <param name="response">Http response</param>
		/// <param name="path">File path</param>
		/// <example>
		/// <code language="cs">
		/// var path = Path.GetTempFileName();
		/// File.WriteAllText(path, "file contents");
		/// HttpManager.CurrentContext.Response.WriteFile(path);
		/// </code>
		/// </example>
		public static void WriteFile(this IHttpResponse response, string path) {
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				stream.CopyTo(response.Body);
			}
			response.Body.Flush();
		}
	}
}
