using Newtonsoft.Json;
using System;
using System.IO;
using ZKWebStandard.Web;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Http回应类的扩展函数
	/// </summary>
	public static class IHttpResponseExtensions {
		/// <summary>
		/// 通过脚本跳转到指定url
		/// 用这个代替301跳转可以保留referer
		/// </summary>
		/// <param name="response">Http回应</param>
		/// <param name="url">跳转到的url地址</param>
		public static void RedirectByScript(this IHttpResponse response, string url) {
			var urlJson = JsonConvert.SerializeObject(url);
			if (response.HasStarted) {
				throw new NotSupportedException("response has started");
			}
			response.ContentType = "text/html";
			response.Write($@"<script type='text/javascript'>location.href = {urlJson};</script>");
			response.Body.Flush();
			response.End();
		}

		/// <summary>
		/// 设置最后修改的时间
		/// </summary>
		/// <param name="response">回应</param>
		/// <param name="date">时间</param>
		public static void SetLastModified(this IHttpResponse response, DateTime date) {
			if (date.Kind != DateTimeKind.Utc) {
				date = date.ToUniversalTime();
			}
			var value = date.ToString("ddd, dd MMM yyyy HH:mm:ss") + " UTC";
			response.AddHeader("Last-Modified", value);
		}

		/// <summary>
		/// 写入字符串到回应
		/// </summary>
		/// <param name="response">Http回应</param>
		/// <param name="value">字符串值</param>
		public static void Write(this IHttpResponse response, string value) {
			var writer = new StreamWriter(response.Body);
			writer.Write(value);
			writer.Flush();
		}

		/// <summary>
		/// 写入文件到回应
		/// </summary>
		/// <param name="response">Http回应</param>
		/// <param name="path">文件路径</param>
		public static void WriteFile(this IHttpResponse response, string path) {
			using (var stream = new FileStream(path, FileMode.Open)) {
				stream.CopyTo(response.Body);
			}
			response.Body.Flush();
		}
	}
}
