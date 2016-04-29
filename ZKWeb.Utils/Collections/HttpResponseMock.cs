using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ZKWeb.Utils.Collections {
	/// <summary>
	/// http回应的模拟类
	/// </summary>
	public class HttpResponseMock : HttpResponseBase {
		public bool buffer = false;
		public bool bufferOutput = false;
		public HttpCachePolicyBase cache = null;
		public string cacheControl = null;
		public string charset = "utf-8";
		public CancellationToken clientDisconnectedToken = new CancellationToken();
		public Encoding contentEncoding = Encoding.UTF8;
		public string contentType = null;
		public HttpCookieCollection cookies = new HttpCookieCollection();
		public int expires = 0;
		public DateTime expiresAbsolute = DateTime.UtcNow;
		public Stream filter = new MemoryStream();
		public Encoding headerEncoding = Encoding.UTF32;
		public NameValueCollection headers = new NameValueCollection();
		public bool headersWritten = false;
		public bool isClientConnected = false;
		public bool isRequestBeingRedirected = false;
		public TextWriter output = new StringWriter();
		public Stream outputStream = new MemoryStream();
		public string redirectLocation = null;
		public string status = null;
		public int statusCode = 0;
		public string statusDescription = null;
		public int subStatusCode = 0;
		public bool supportsAsyncFlush = false;
		public bool suppressContent = false;
		public bool suppressDefaultCacheControlHeader = false;
		public bool suppressFormsAuthenticationRedirect = false;
		public bool trySkipIisCustomErrors = false;

		public override bool Buffer {
			get { return buffer; }
			set { buffer = value; }
		}
		public override bool BufferOutput {
			get { return bufferOutput; }
			set { bufferOutput = value; }
		}
		public override HttpCachePolicyBase Cache {
			get { return cache; }
		}
		public override string CacheControl {
			get { return cacheControl; }
			set { cacheControl = value; }
		}
		public override string Charset {
			get { return charset; }
			set { charset = value; }
		}
		public override CancellationToken ClientDisconnectedToken { get { return clientDisconnectedToken; } }
		public override Encoding ContentEncoding {
			get { return contentEncoding; }
			set { contentEncoding = value; }
		}
		public override string ContentType {
			get { return contentType; }
			set { contentType = value; }
		}
		public override HttpCookieCollection Cookies { get { return cookies; } }
		public override int Expires {
			get { return expires; }
			set { expires = value; }
		}
		public override DateTime ExpiresAbsolute {
			get { return expiresAbsolute; }
			set { expiresAbsolute = value; }
		}
		public override Stream Filter {
			get { return filter; }
			set { filter = value; }
		}
		public override Encoding HeaderEncoding {
			get { return headerEncoding; }
			set { headerEncoding = value; }
		}
		public override NameValueCollection Headers { get { return headers; } }
		public override bool HeadersWritten { get { return headersWritten; } }
		public override bool IsClientConnected { get { return isClientConnected; } }
		public override bool IsRequestBeingRedirected { get { return isRequestBeingRedirected; } }
		public override TextWriter Output {
			get { return output; }
			set { output = value; }
		}
		public override Stream OutputStream { get { return outputStream; } }
		public override string RedirectLocation {
			get { return redirectLocation; }
			set { redirectLocation = value; }
		}
		public override string Status {
			get { return status; }
			set { status = value; }
		}
		public override int StatusCode {
			get { return statusCode; }
			set { statusCode = value; }
		}
		public override string StatusDescription {
			get { return statusDescription; }
			set { statusDescription = value; }
		}
		public override int SubStatusCode {
			get { return subStatusCode; }
			set { subStatusCode = value; }
		}
		public override bool SupportsAsyncFlush { get { return supportsAsyncFlush; } }
		public override bool SuppressContent {
			get { return suppressContent; }
			set { suppressContent = value; }
		}
		public override bool SuppressDefaultCacheControlHeader {
			get { return suppressDefaultCacheControlHeader; }
			set { suppressDefaultCacheControlHeader = value; }
		}
		public override bool SuppressFormsAuthenticationRedirect {
			get { return suppressFormsAuthenticationRedirect; }
			set { suppressFormsAuthenticationRedirect = value; }
		}
		public override bool TrySkipIisCustomErrors {
			get { return trySkipIisCustomErrors; }
			set { trySkipIisCustomErrors = value; }
		}
	}
}
