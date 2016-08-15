using System.IO;

namespace ZKWebStandard.Web.Wrappers {
	/// <summary>
	/// Http response wrapper base class
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
#pragma warning restore CS1591

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="originalResponse">原始的Http回应</param>
		public HttpResponseWrapper(IHttpResponse originalResponse) {
			OriginalResponse = originalResponse;
		}
	}
}
