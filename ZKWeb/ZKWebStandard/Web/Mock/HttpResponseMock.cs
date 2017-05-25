using System.Collections.Generic;
using System.IO;
using ZKWebStandard.Extensions;

namespace ZKWebStandard.Web.Mock {
	/// <summary>
	/// Http response mock class<br/>
	/// Http回应的模拟类<br/>
	/// </summary>
	public class HttpResponseMock : IHttpResponse {
#pragma warning disable CS1591
		public Stream body { get; set; }
		public IHttpContext httpContext { get; set; }
		public bool isEnd { get; set; }
		public IDictionary<string, string> cookies { get; set; }
		public IDictionary<string, IList<string>> headers { get; set; }
		public string lastRedirect { get; set; }
		public bool lastRedirectIsPermanent { get; set; }

		public virtual Stream Body { get { return body; } }
		public virtual string ContentType { get; set; }
		public virtual IHttpContext HttpContext { get { return httpContext; } }
		public virtual int StatusCode { get; set; }

		public HttpResponseMock(IHttpContext context) {
			body = new MemoryStream();
			httpContext = context;
			isEnd = false;
			cookies = new Dictionary<string, string>();
			headers = new Dictionary<string, IList<string>>();
			lastRedirect = null;
			lastRedirectIsPermanent = false;
		}

		public virtual void End() {
			Body.Flush();
			isEnd = true;
		}

		public virtual void SetCookie(string key, string value, HttpCookieOptions options) {
			cookies[key] = value;
		}

		public virtual void AddHeader(string key, string value) {
			headers.GetOrCreate(key, () => new List<string>()).Add(value);
		}

		public virtual void Redirect(string url, bool permanent) {
			lastRedirect = url;
			lastRedirectIsPermanent = permanent;
		}

		public virtual string GetContentsFromBody() {
			Body.Seek(0, SeekOrigin.Begin);
			return new StreamReader(Body).ReadToEnd();
		}
#pragma warning restore CS1591
	}
}
