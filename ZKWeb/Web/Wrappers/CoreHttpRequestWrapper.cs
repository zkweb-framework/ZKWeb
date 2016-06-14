using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using ZKWebStandard.Collections;
using ZKWebStandard.Web;

namespace ZKWeb.Web.Wrappers {
	/// <summary>
	/// 包装AspNetCore的Http请求
	/// </summary>
	internal class CoreHttpRequestWrapper : IHttpRequest {
		/// <summary>
		/// 所属的Http上下文
		/// </summary>
		protected CoreHttpContextWrapper ParentContext { get; set; }
		/// <summary>
		/// AspNetCore的Http请求
		/// </summary>
		protected HttpRequest CoreRequest { get; set; }

		public Stream Body {
			get { return CoreRequest.Body; }
		}
		public long? ContentLength {
			get { return CoreRequest.ContentLength; }
		}
		public string Host {
			get { return CoreRequest.Host.ToString(); }
		}
		public IHttpContext HttpContext {
			get { return ParentContext; }
		}
		public bool IsHttps {
			get { return CoreRequest.IsHttps; }
		}
		public string Method {
			get { return CoreRequest.Method; }
		}
		public string Protocol {
			get { return CoreRequest.Protocol; }
		}
		public string Path {
			get { return CoreRequest.Path; }
		}
		public string QueryString {
			get { return CoreRequest.QueryString.ToString(); }
		}
		public string Scheme {
			get { return CoreRequest.Scheme; }
		}
		public X509Certificate2 ClientCertificate {
			get { return ParentContext.Connection.ClientCertificate; }
		}
		public IPAddress LocalIpAddress {
			get { return ParentContext.Connection.LocalIpAddress; }
		}
		public int LocalPort {
			get { return ParentContext.Connection.LocalPort; }
		}
		public IPAddress RemoteIpAddress {
			get { return ParentContext.Connection.RemoteIpAddress; }
		}
		public int RemotePort {
			get { return ParentContext.Connection.RemotePort; }
		}

		public string GetCookie(string key) {
			return CoreRequest.Cookies[key];
		}
		public IEnumerable<Pair<string, string>> GetCookies() {
			foreach (var pair in CoreRequest.Cookies) {
				yield return Pair.Create(pair.Key, pair.Value);
			}
		}
		public IList<string> GetQueryValue(string key) {
			return CoreRequest.Query[key];
		}
		public IEnumerable<Pair<string, IList<string>>> GetQueryValues() {
			foreach (var pair in CoreRequest.Query) {
				yield return Pair.Create<string, IList<string>>(pair.Key, pair.Value);
			}
		}
		public IList<string> GetFormValue(string key) {
			return CoreRequest.Form[key];
		}
		public IEnumerable<Pair<string, IList<string>>> GetFormValues() {
			foreach (var pair in CoreRequest.Form) {
				yield return Pair.Create<string, IList<string>>(pair.Key, pair.Value);
			}
		}
		public string GetHeader(string key) {
			// http://stackoverflow.com/questions/4371328/are-duplicate-http-response-headers-acceptable
			IList<string> values = CoreRequest.Headers[key];
			if (values == null) {
				return null;
			}
			return string.Join(",", values);
		}
		public IEnumerable<Pair<string, string>> GetHeaders() {
			foreach (var pair in CoreRequest.Headers) {
				yield return Pair.Create(pair.Key, string.Join(",", pair.Value[0]));
			}
		}
		public IHttpPostedFile GetPostedFile(string key) {
			var coreFile = CoreRequest.Form.Files[key];
			if (coreFile == null) {
				return null;
			}
			return new CoreHttpPostedFileWrapper(coreFile);
		}
		public IEnumerable<Pair<string, IHttpPostedFile>> GetPostedFiles() {
			foreach (var coreFile in CoreRequest.Form.Files) {
				yield return Pair.Create<string, IHttpPostedFile>(
					coreFile.Name, new CoreHttpPostedFileWrapper(coreFile));
			}
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="parentContext">所属的Http上下文</param>
		/// <param name="coreRequest">AspNetCore的Http请求</param>
		public CoreHttpRequestWrapper(
			CoreHttpContextWrapper parentContext, HttpRequest coreRequest) {
			ParentContext = parentContext;
			CoreRequest = coreRequest;
		}
	}
}
