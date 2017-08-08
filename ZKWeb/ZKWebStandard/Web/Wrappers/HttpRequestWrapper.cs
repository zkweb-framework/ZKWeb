using System.Collections.Generic;
using System.IO;
using System.Net;
using ZKWebStandard.Collections;

namespace ZKWebStandard.Web.Wrappers {
	/// <summary>
	/// Http request wrapper base class<br/>
	/// Http请求包装类的基类<br/>
	/// </summary>
	public abstract class HttpRequestWrapper : IHttpRequest {
#pragma warning disable CS1591
		protected IHttpRequest OriginalRequest { get; set; }

		public virtual Stream Body {
			get { return OriginalRequest.Body; }
		}
		public virtual long? ContentLength {
			get { return OriginalRequest.ContentLength; }
		}
		public virtual string ContentType {
			get { return OriginalRequest.ContentType; }
		}
		public virtual string Host {
			get { return OriginalRequest.Host; }
		}
		public virtual IHttpContext HttpContext {
			get { return OriginalRequest.HttpContext; }
		}
		public virtual bool IsHttps {
			get { return OriginalRequest.IsHttps; }
		}
		public virtual string Method {
			get { return OriginalRequest.Method; }
		}
		public virtual string Path {
			get { return OriginalRequest.Path; }
		}
		public virtual string Protocol {
			get { return OriginalRequest.Protocol; }
		}
		public virtual string QueryString {
			get { return OriginalRequest.QueryString; }
		}
		public virtual IPAddress RemoteIpAddress {
			get { return OriginalRequest.RemoteIpAddress; }
		}
		public virtual int RemotePort {
			get { return OriginalRequest.RemotePort; }
		}
		public virtual string Scheme {
			get { return OriginalRequest.Scheme; }
		}
		public virtual IDictionary<string, object> CustomParameters {
			get { return OriginalRequest.CustomParameters; }
		}
		public virtual string GetCookie(string key) {
			return OriginalRequest.GetCookie(key);
		}
		public virtual IEnumerable<Pair<string, string>> GetCookies() {
			return OriginalRequest.GetCookies();
		}
		public virtual IList<string> GetFormValue(string key) {
			return OriginalRequest.GetFormValue(key);
		}
		public virtual IEnumerable<Pair<string, IList<string>>> GetFormValues() {
			return OriginalRequest.GetFormValues();
		}
		public virtual string GetHeader(string key) {
			return OriginalRequest.GetHeader(key);
		}
		public virtual IEnumerable<Pair<string, string>> GetHeaders() {
			return OriginalRequest.GetHeaders();
		}
		public virtual IList<string> GetQueryValue(string key) {
			return OriginalRequest.GetQueryValue(key);
		}
		public virtual IEnumerable<Pair<string, IList<string>>> GetQueryValues() {
			return OriginalRequest.GetQueryValues();
		}
		public virtual IHttpPostedFile GetPostedFile(string key) {
			return OriginalRequest.GetPostedFile(key);
		}
		public virtual IEnumerable<Pair<string, IHttpPostedFile>> GetPostedFiles() {
			return OriginalRequest.GetPostedFiles();
		}

		public HttpRequestWrapper(IHttpRequest originalRequest) {
			OriginalRequest = originalRequest;
		}
#pragma warning restore CS1591
	}
}
