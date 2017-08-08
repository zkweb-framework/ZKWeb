using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

namespace ZKWeb.Hosting.Owin {
	/// <summary>
	/// Http request wrapper for Owin<br/>
	/// Owin的Http请求的包装类<br/>
	/// </summary>
	internal class OwinHttpRequestWrapper : IHttpRequest {
		/// <summary>
		/// Parent http context<br/>
		/// 所属的Http上下文<br/>
		/// </summary>
		protected OwinHttpContextWrapper ParentContext { get; set; }
		/// <summary>
		/// Original http request<br/>
		/// 原始的Http请求<br/>
		/// </summary>
		protected IOwinRequest OwinRequest { get; set; }
		/// <summary>
		/// Http content object, used for read form contents<br/>
		/// Http内容对象, 用于读取内容<br/>
		/// </summary>
		protected HttpContent OwinContent { get; set; }
		/// <summary>
		/// Owin form collection<br/>
		/// Owin的表单集合<br/>
		/// </summary>
		protected Lazy<NameValueCollection> OwinFormCollection { get; set; }
		/// <summary>
		/// Owin multipart form collection<br/>
		/// Owin的multipart表单集合<br/>
		/// </summary>
		protected Lazy<Dictionary<string, IList<HttpContent>>> OwinMultipartFormCollection { get; set; }
		/// <summary>
		/// Read contents<br/>
		/// Because contents can only read once, it need a variable to store previous read result<br/>
		/// 已读取的内容<br/>
		/// 因为内容只能读取一次, 需要一个值来保存之前读取的结果<br/>
		/// </summary>
		protected Dictionary<HttpContent, string> HttpContentReadResults { get; set; }

		public Stream Body {
			get { return OwinRequest.Body; }
		}
		public long? ContentLength {
			get { return OwinRequest.Headers["Content-Length"].ConvertOrDefault<long?>(); }
		}
		public string ContentType {
			get { return OwinRequest.ContentType; }
		}
		public string Host {
			get { return OwinRequest.Uri.Authority; }
		}
		public IHttpContext HttpContext {
			get { return ParentContext; }
		}
		public bool IsHttps {
			get { return OwinRequest.IsSecure; }
		}
		public string Method {
			get { return OwinRequest.Method; }
		}
		public string Protocol {
			get { return OwinRequest.Protocol; }
		}
		public string Path {
			get { return OwinRequest.Uri.AbsolutePath; }
		}
		public string QueryString {
			get { return OwinRequest.Uri.Query; }
		}
		public string Scheme {
			get { return OwinRequest.Scheme; }
		}
		public IPAddress RemoteIpAddress {
			get { return IPAddress.Parse(OwinRequest.RemoteIpAddress); }
		}
		public int RemotePort {
			get { return OwinRequest.RemotePort ?? 0; }
		}
		public IDictionary<string, object> CustomParameters { get; }

		public string GetCookie(string key) {
			return OwinRequest.Cookies[key];
		}
		public IEnumerable<Pair<string, string>> GetCookies() {
			foreach (var pair in OwinRequest.Cookies) {
				yield return Pair.Create(pair.Key, pair.Value);
			}
		}
		public IList<string> GetQueryValue(string key) {
			return OwinRequest.Query.GetValues(key);
		}
		public IEnumerable<Pair<string, IList<string>>> GetQueryValues() {
			foreach (var pair in OwinRequest.Query) {
				yield return Pair.Create<string, IList<string>>(pair.Key, pair.Value);
			}
		}
		protected string ReadHttpContentAsString(HttpContent content) {
			return HttpContentReadResults.GetOrCreate(
				content, () => content.ReadAsStringAsync().Result);
		}
		public IList<string> GetFormValue(string key) {
			if (OwinFormCollection.Value != null) {
				// From url encoded form
				return OwinFormCollection.Value.GetValues(key);
			} else if (OwinMultipartFormCollection.Value != null) {
				// From multi part form
				var contents = OwinMultipartFormCollection.Value.GetOrDefault(key);
				if (contents != null) {
					return contents.Select(c => ReadHttpContentAsString(c)).ToList();
				}
			}
			return null;
		}
		public IEnumerable<Pair<string, IList<string>>> GetFormValues() {
			if (OwinFormCollection.Value != null) {
				// From url encoded form
				foreach (var key in OwinFormCollection.Value.AllKeys) {
					yield return Pair.Create<string, IList<string>>(
						key, OwinFormCollection.Value.GetValues(key));
				}
			} else if (OwinMultipartFormCollection.Value != null) {
				// From multi part form
				foreach (var pair in OwinMultipartFormCollection.Value) {
					if (pair.Value.Any(
						c => !string.IsNullOrEmpty(c.Headers.ContentDisposition.FileName))) {
						continue; // Ignore files
					}
					yield return Pair.Create<string, IList<string>>(
						pair.Key, pair.Value.Select(c => ReadHttpContentAsString(c)).ToList());
				}
			}
		}
		public string GetHeader(string key) {
			// http://stackoverflow.com/questions/4371328/are-duplicate-http-response-headers-acceptable
			IList<string> values = OwinRequest.Headers.GetValues(key);
			if (values == null) {
				return null;
			}
			return string.Join(",", values);
		}
		public IEnumerable<Pair<string, string>> GetHeaders() {
			foreach (var pair in OwinRequest.Headers) {
				yield return Pair.Create(pair.Key, string.Join(",", pair.Value[0]));
			}
		}
		public IHttpPostedFile GetPostedFile(string key) {
			if (OwinMultipartFormCollection.Value != null) {
				// From multi part form
				var values = OwinMultipartFormCollection.Value.GetOrDefault(key);
				if (values != null) {
					var content = values[0];
					return new OwinHttpPostedFileWrapper(content);
				}
			}
			return null;
		}
		public IEnumerable<Pair<string, IHttpPostedFile>> GetPostedFiles() {
			if (OwinMultipartFormCollection.Value != null) {
				// From multi part form
				foreach (var pair in OwinMultipartFormCollection.Value) {
					var content = pair.Value[0];
					yield return Pair.Create<string, IHttpPostedFile>(
						pair.Key, new OwinHttpPostedFileWrapper(content));
				}
			}
		}

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="parentContext">Parent context</param>
		/// <param name="owinRequest">Original http request</param>
		public OwinHttpRequestWrapper(
			OwinHttpContextWrapper parentContext, IOwinRequest owinRequest) {
			ParentContext = parentContext;
			OwinRequest = owinRequest;
			OwinContent = new StreamContent(OwinRequest.Body);
			var contentType = OwinRequest.ContentType ?? "";
			if (!string.IsNullOrEmpty(contentType)) {
				OwinContent.Headers.Add("Content-Type", contentType);
			}
			OwinFormCollection = new Lazy<NameValueCollection>(() => {
				if (contentType.StartsWith("application/x-www-form-urlencoded")) {
					return OwinContent.ReadAsFormDataAsync().Result;
				}
				return null;
			});
			OwinMultipartFormCollection = new Lazy<Dictionary<string, IList<HttpContent>>>(() => {
				if (contentType.StartsWith("multipart/form-data")) {
					var result = new Dictionary<string, IList<HttpContent>>();
					var provider = OwinContent.ReadAsMultipartAsync().Result;
					foreach (var content in provider.Contents) {
						var key = content.Headers.ContentDisposition.Name.Trim('"');
						result.GetOrCreate(key, () => new List<HttpContent>()).Add(content);
					}
					return result;
				}
				return null;
			});
			HttpContentReadResults = new Dictionary<HttpContent, string>();
			CustomParameters = new Dictionary<string, object>();
		}
	}
}
