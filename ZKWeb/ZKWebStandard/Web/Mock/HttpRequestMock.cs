using System.Collections.Generic;
using System.IO;
using System.Net;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;

namespace ZKWebStandard.Web.Mock {
	/// <summary>
	/// Http request mock class<br/>
	/// Http请求的模拟类<br/>
	/// </summary>
	public class HttpRequestMock : IHttpRequest {
#pragma warning disable CS1591
		public Stream body { get; set; }
		public long? contentLength { get; set; }
		public string contentType { get; set; }
		public string host { get; set; }
		public IHttpContext httpContext { get; set; }
		public bool isHttps { get; set; }
		public string method { get; set; }
		public string path { get; set; }
		public string protocol { get; set; }
		public string queryString { get; set; }
		public IPAddress remoteIpAddress { get; set; }
		public int remotePort { get; set; }
		public string scheme { get; set; }
		public IDictionary<string, string> cookies { get; set; }
		public IDictionary<string, IList<string>> form { get; set; }
		public IDictionary<string, string> headers { get; set; }
		public IDictionary<string, IList<string>> query { get; set; }
		public IDictionary<string, IHttpPostedFile> postedFiles { get; set; }
		public IDictionary<string, object> customParameters { get; set; }

		public virtual Stream Body { get { return body; } }
		public virtual long? ContentLength { get { return contentLength; } }
		public virtual string ContentType { get { return contentType; } }
		public virtual string Host { get { return host; } }
		public virtual IHttpContext HttpContext { get { return httpContext; } }
		public virtual bool IsHttps { get { return isHttps; } }
		public virtual string Method { get { return method; } }
		public virtual string Path { get { return path; } }
		public virtual string Protocol { get { return protocol; } }
		public virtual string QueryString { get { return queryString; } }
		public virtual IPAddress RemoteIpAddress { get { return remoteIpAddress; } }
		public virtual int RemotePort { get { return remotePort; } }
		public virtual string Scheme { get { return Scheme; } }
		public virtual IDictionary<string, object> CustomParameters { get { return customParameters; } }

		public HttpRequestMock(IHttpContext context) {
			body = new MemoryStream();
			contentLength = null;
			contentType = null;
			host = "localhost";
			httpContext = context;
			isHttps = false;
			method = "GET";
			path = "/";
			protocol = "HTTP/1.1";
			queryString = "";
			remoteIpAddress = IPAddress.Loopback;
			remotePort = 65535;
			scheme = "http";
			cookies = new Dictionary<string, string>();
			form = new Dictionary<string, IList<string>>();
			headers = new Dictionary<string, string>();
			query = new Dictionary<string, IList<string>>();
			postedFiles = new Dictionary<string, IHttpPostedFile>();
			customParameters = new Dictionary<string, object>();
		}

		public virtual string GetCookie(string key) {
			return cookies.GetOrDefault(key);
		}

		public virtual IEnumerable<Pair<string, string>> GetCookies() {
			foreach (var pair in cookies) {
				yield return Pair.Create(pair.Key, pair.Value);
			}
		}

		public virtual IList<string> GetFormValue(string key) {
			return form.GetOrDefault(key);
		}

		public virtual IEnumerable<Pair<string, IList<string>>> GetFormValues() {
			foreach (var pair in form) {
				yield return Pair.Create(pair.Key, pair.Value);
			}
		}

		public virtual string GetHeader(string key) {
			return headers.GetOrDefault(key);
		}

		public virtual IEnumerable<Pair<string, string>> GetHeaders() {
			foreach (var pair in headers) {
				yield return Pair.Create(pair.Key, pair.Value);
			}
		}

		public virtual IList<string> GetQueryValue(string key) {
			return query.GetOrDefault(key);
		}

		public virtual IEnumerable<Pair<string, IList<string>>> GetQueryValues() {
			foreach (var pair in query) {
				yield return Pair.Create(pair.Key, pair.Value);
			}
		}

		public virtual IHttpPostedFile GetPostedFile(string key) {
			return postedFiles.GetOrDefault(key);
		}

		public virtual IEnumerable<Pair<string, IHttpPostedFile>> GetPostedFiles() {
			foreach (var pair in postedFiles) {
				yield return Pair.Create(pair.Key, pair.Value);
			}
		}
#pragma warning restore CS1591
	}
}
