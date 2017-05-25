using System.Collections.Generic;
using ZKWebStandard.Utils;

namespace ZKWebStandard.Web.Mock {
	/// <summary>
	/// Http context mock class<br/>
	/// Http上下文的模拟类<br/>
	/// </summary>
	public class HttpContextMock : IHttpContext {
#pragma warning disable CS1591
		public HttpRequestMock request { get; set; }
		public HttpResponseMock response { get; set; }
		public IDictionary<object, object> items { get; set; }

		public HttpContextMock() {
			request = new HttpRequestMock(this);
			response = new HttpResponseMock(this);
			items = new Dictionary<object, object>();
		}

		public HttpContextMock(string pathAndQuery, string method) : this() {
			string path;
			string queryString;
			HttpUtils.SplitPathAndQuery(pathAndQuery, out path, out queryString);
			if (!path.StartsWith("/")) {
				path = "/" + path;
			}
			request.path = path;
			if (method == "GET") {
				request.queryString = queryString;
				request.query = HttpUtils.ParseQueryString(queryString);
			} else {
				request.contentType = "application/x-www-form-urlencoded";
				request.form = HttpUtils.ParseQueryString(queryString);
			}
			request.method = method;
		}

		public virtual IDictionary<object, object> Items { get { return items; } }
		public virtual IHttpRequest Request { get { return request; } }
		public virtual IHttpResponse Response { get { return response; } }
#pragma warning restore CS1591
	}
}
