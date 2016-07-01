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
	/// 包装Owin的Http请求
	/// </summary>
	internal class OwinHttpRequestWrapper : IHttpRequest {
		/// <summary>
		/// 所属的Http上下文
		/// </summary>
		protected OwinHttpContextWrapper ParentContext { get; set; }
		/// <summary>
		/// Owin的Http请求
		/// </summary>
		protected IOwinRequest OwinRequest { get; set; }
		/// <summary>
		/// Owin的Http请求内容，用于读取表单内容
		/// </summary>
		protected HttpContent OwinContent { get; set; }
		/// <summary>
		/// Owin的表单内容
		/// </summary>
		protected Lazy<NameValueCollection> OwinFormCollection { get; set; }
		/// <summary>
		/// Owin的带文件的表单内容
		/// </summary>
		protected Lazy<Dictionary<string, IList<HttpContent>>> OwinMultipartFormCollection { get; set; }
		/// <summary>
		/// 表单内容读取到字符串的结果
		/// 因为只能读取一次，之前读取的值需要保存在这里
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
				// 从普通的表单获取
				return OwinFormCollection.Value.GetValues(key);
			} else if (OwinMultipartFormCollection.Value != null) {
				// 从带文件的表单获取
				var contents = OwinMultipartFormCollection.Value.GetOrDefault(key);
				if (contents != null) {
					return contents.Select(c => ReadHttpContentAsString(c)).ToList();
				}
			}
			return null;
		}
		public IEnumerable<Pair<string, IList<string>>> GetFormValues() {
			if (OwinFormCollection.Value != null) {
				// 从普通的表单获取
				foreach (var key in OwinFormCollection.Value.AllKeys) {
					yield return Pair.Create<string, IList<string>>(
						key, OwinFormCollection.Value.GetValues(key));
				}
			} else if (OwinMultipartFormCollection.Value != null) {
				// 从带文件的表单获取
				foreach (var pair in OwinMultipartFormCollection.Value) {
					if (pair.Value.Any(
						c => !string.IsNullOrEmpty(c.Headers.ContentDisposition.FileName))) {
						continue; // 跳过文件
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
				// 从带文件的表单获取
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
				// 从带文件的表单获取
				foreach (var pair in OwinMultipartFormCollection.Value) {
					var content = pair.Value[0];
					yield return Pair.Create<string, IHttpPostedFile>(
						pair.Key, new OwinHttpPostedFileWrapper(content));
				}
			}
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="parentContext">所属的Http上下文</param>
		/// <param name="owinRequest">Owin的Http请求</param>
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
		}
	}
}
