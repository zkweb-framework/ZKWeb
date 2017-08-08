using System.Collections.Generic;
using System.IO;
using System.Net;
using ZKWebStandard.Collections;
using ZKWebStandard.Web;
using System.Web;

namespace ZKWeb.Hosting.AspNet {
	/// <summary>
	/// Http request wrapper for Asp.net<br/>
	/// Asp.Net Http请求的包装类<br/>
	/// </summary>
	internal class AspNetHttpRequestWrapper : IHttpRequest {
		/// <summary>
		/// Parent http context<br/>
		/// 所属的Http上下文<br/>
		/// </summary>
		protected AspNetHttpContextWrapper ParentContext { get; set; }
		/// <summary>
		/// Original http request<br/>
		/// 原始的Http请求<br/>
		/// </summary>
		protected HttpRequest OriginalRequest { get; set; }

		public Stream Body {
			get { return OriginalRequest.InputStream; }
		}
		public long? ContentLength {
			get { return OriginalRequest.ContentLength; }
		}
		public string ContentType {
			get { return OriginalRequest.ContentType; }
		}
		public string Host {
			get { return OriginalRequest.Url.Authority; }
		}
		public IHttpContext HttpContext {
			get { return ParentContext; }
		}
		public bool IsHttps {
			get { return OriginalRequest.IsSecureConnection; }
		}
		public string Method {
			get { return OriginalRequest.HttpMethod; }
		}
		public string Protocol {
			get { return OriginalRequest.ServerVariables["SERVER_PROTOCOL"]; }
		}
		public string Path {
			get { return OriginalRequest.Path; }
		}
		public string QueryString {
			get { return OriginalRequest.Url.Query; }
		}
		public string Scheme {
			get { return OriginalRequest.Url.Scheme; }
		}
		public IPAddress RemoteIpAddress {
			get { return IPAddress.Parse(OriginalRequest.ServerVariables["REMOTE_ADDR"]); }
		}
		public int RemotePort {
			get { return int.Parse(OriginalRequest.ServerVariables["REMOTE_PORT"]); }
		}
		public IDictionary<string, object> CustomParameters { get; }

		public string GetCookie(string key) {
			return OriginalRequest.Cookies[key]?.Value;
		}
		public IEnumerable<Pair<string, string>> GetCookies() {
			foreach (Cookie cookie in OriginalRequest.Cookies) {
				yield return Pair.Create(cookie.Name, cookie.Value);
			}
		}
		public IList<string> GetQueryValue(string key) {
			return OriginalRequest.QueryString.GetValues(key);
		}
		public IEnumerable<Pair<string, IList<string>>> GetQueryValues() {
			var query = OriginalRequest.QueryString;
			foreach (var key in query.AllKeys) {
				yield return Pair.Create<string, IList<string>>(key, query.GetValues(key));
			}
		}
		public IList<string> GetFormValue(string key) {
			return OriginalRequest.Form.GetValues(key);
		}
		public IEnumerable<Pair<string, IList<string>>> GetFormValues() {
			var form = OriginalRequest.Form;
			foreach (var key in form.AllKeys) {
				yield return Pair.Create<string, IList<string>>(key, form.GetValues(key));
			}
		}
		public string GetHeader(string key) {
			// http://stackoverflow.com/questions/4371328/are-duplicate-http-response-headers-acceptable
			IList<string> values = OriginalRequest.Headers.GetValues(key);
			if (values == null) {
				return null;
			}
			return string.Join(",", values);
		}
		public IEnumerable<Pair<string, string>> GetHeaders() {
			var headers = OriginalRequest.Headers;
			foreach (var key in headers.AllKeys) {
				yield return Pair.Create(key, string.Join(",", headers.GetValues(key)));
			}
		}
		public IHttpPostedFile GetPostedFile(string key) {
			var file = OriginalRequest.Files[key];
			if (file == null) {
				return null;
			}
			return new AspNetHttpPostedFileWrapper(file);
		}
		public IEnumerable<Pair<string, IHttpPostedFile>> GetPostedFiles() {
			var files = OriginalRequest.Files;
			foreach (var key in files.AllKeys) {
				yield return Pair.Create<string, IHttpPostedFile>(
					key, new AspNetHttpPostedFileWrapper(files[key]));
			}
		}

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="parentContext">Parent http context</param>
		/// <param name="originalRequest">Original http request</param>
		public AspNetHttpRequestWrapper(
			AspNetHttpContextWrapper parentContext, HttpRequest originalRequest) {
			ParentContext = parentContext;
			OriginalRequest = originalRequest;
			CustomParameters = new Dictionary<string, object>();
		}
	}
}
