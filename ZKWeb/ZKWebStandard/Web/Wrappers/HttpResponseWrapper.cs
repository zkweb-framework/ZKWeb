using System.IO;

namespace ZKWebStandard.Web.Wrappers {
	/// <summary>
	/// Http response wrapper base class<br/>
	/// Http回应包装类的基类<br/>
	/// </summary>
	public abstract class HttpResponseWrapper : IHttpResponse {
#pragma warning disable CS1591
		protected IHttpResponse OriginalResponse { get; set; }

		public virtual Stream Body {
			get { return OriginalResponse.Body; }
		}
		public virtual string ContentType {
			get { return OriginalResponse.ContentType; }
			set { OriginalResponse.ContentType = value; }
		}
		public virtual IHttpContext HttpContext {
			get { return OriginalResponse.HttpContext; }
		}
		public virtual int StatusCode {
			get { return OriginalResponse.StatusCode; }
			set { OriginalResponse.StatusCode = value; }
		}
		public virtual void End() {
			OriginalResponse.End();
		}
		public virtual void SetCookie(string key, string value, HttpCookieOptions options) {
			OriginalResponse.SetCookie(key, value, options);
		}
		public virtual void AddHeader(string key, string value) {
			OriginalResponse.AddHeader(key, value);
		}
		public virtual void Redirect(string url, bool permanent) {
			OriginalResponse.Redirect(url, permanent);
		}

		public HttpResponseWrapper(IHttpResponse originalResponse) {
			OriginalResponse = originalResponse;
		}
#pragma warning restore CS1591
	}
}
