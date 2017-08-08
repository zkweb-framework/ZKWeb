using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Http;
using ZKWebStandard.Collections;
using ZKWebStandard.Web;
using System;

namespace ZKWeb.Hosting.AspNetCore {
	/// <summary>
	/// Http request wrapper for Asp.net Core<br/>
	/// Asp.net Core的Http请求包装类<br/>
	/// </summary>
	internal class CoreHttpRequestWrapper : IHttpRequest {
		/// <summary>
		/// Parent http context<br/>
		/// 所属的Http上下文<br/>
		/// </summary>
		protected CoreHttpContextWrapper ParentContext { get; set; }
		/// <summary>
		/// Original http request<br/>
		/// 原始的Http请求<br/>
		/// </summary>
		protected HttpRequest CoreRequest { get; set; }
		/// <summary>
		/// Detect request contains form values<br/>
		/// It's necessary for Asp.Net Core<br/>
		/// And because ContentType ain't arrived when construct, it should be a lazy value<br/>
		/// 检测请求是否包含表单值<br/>
		/// Asp.Net Core需要这项检查<br/>
		/// 并且因为构建这个实例时ContentType尚未收到, 它应该是一个懒值<br/>
		/// </summary>
		protected Lazy<bool> ContainsFormValues { get; set; }

		public Stream Body {
			get { return CoreRequest.Body; }
		}
		public long? ContentLength {
			get { return CoreRequest.ContentLength; }
		}
		public string ContentType {
			get { return CoreRequest.ContentType; }
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
		public IPAddress RemoteIpAddress {
			get { return ParentContext.Connection.RemoteIpAddress; }
		}
		public int RemotePort {
			get { return ParentContext.Connection.RemotePort; }
		}
		public IDictionary<string, object> CustomParameters { get; }

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
			if (!ContainsFormValues.Value) {
				return null;
			}
			return CoreRequest.Form[key];
		}
		public IEnumerable<Pair<string, IList<string>>> GetFormValues() {
			if (!ContainsFormValues.Value) {
				yield break;
			}
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
			if (!ContainsFormValues.Value) {
				return null;
			}
			var coreFile = CoreRequest.Form.Files[key];
			if (coreFile == null) {
				return null;
			}
			return new CoreHttpPostedFileWrapper(coreFile);
		}
		public IEnumerable<Pair<string, IHttpPostedFile>> GetPostedFiles() {
			if (!ContainsFormValues.Value) {
				yield break;
			}
			foreach (var coreFile in CoreRequest.Form.Files) {
				yield return Pair.Create<string, IHttpPostedFile>(
					coreFile.Name, new CoreHttpPostedFileWrapper(coreFile));
			}
		}

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="parentContext">Parent context</param>
		/// <param name="coreRequest">Original http request</param>
		public CoreHttpRequestWrapper(
			CoreHttpContextWrapper parentContext, HttpRequest coreRequest) {
			ParentContext = parentContext;
			CoreRequest = coreRequest;
			ContainsFormValues = new Lazy<bool>(() => {
				var contentType = (ContentType ?? "").Split(';')[0];
				return (contentType == "application/x-www-form-urlencoded" ||
					contentType == "multipart/form-data");
			});
			CustomParameters = new Dictionary<string, object>();
		}
	}
}
