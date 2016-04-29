using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Utils.Collections {
	/// <summary>
	/// http请求的模拟类
	/// </summary>
	public class HttpRequestMock : HttpRequestBase {
		public string[] acceptTypes = new string[0];
		public string anonymousID = null;
		public string applicationPath = null;
		public string appRelativeCurrentExecutionFilePath = null;
		public HttpBrowserCapabilitiesBase browser = null;
		public HttpClientCertificate clientCertificate = null;
		public Encoding contentEncoding = Encoding.UTF8;
		public int contentLength = 0;
		public string contentType = null;
		public HttpCookieCollection cookies = new HttpCookieCollection();
		public string currentExecutionFilePath = null;
		public string currentExecutionFilePathExtension = null;
		public string filePath = null;
		public HttpFileCollectionBase files = null;
		public Stream filter = null;
		public NameValueCollection form = new NameValueCollection();
		public NameValueCollection headers = new NameValueCollection();
		public ChannelBinding httpChannelBinding = null;
		public string httpMethod = null;
		public Stream inputStream = new MemoryStream();
		public bool isAuthenticated = false;
		public bool isLocal = true;
		public bool isSecureConnection = false;
		public WindowsIdentity logonUserIdentity = null;
		public NameValueCollection @params = null;
		public string path = null;
		public string pathInfo = null;
		public string physicalApplicationPath = null;
		public string physicalPath = null;
		public NameValueCollection queryString = new NameValueCollection();
		public string rawUrl = null;
		public ReadEntityBodyMode readEntityBodyMode = ReadEntityBodyMode.None;
		public RequestContext requestContext = new RequestContext();
		public string requestType = null;
		public NameValueCollection serverVariables = new NameValueCollection();
		public CancellationToken timedOutToken = new CancellationToken();
		public int totalBytes = 0;
		public UnvalidatedRequestValuesBase unvalidated = null;
		public Uri url = null;
		public Uri urlReferrer = null;
		public string userAgent = null;
		public string userHostAddress = null;
		public string userHostName = null;
		public string[] userLanguages = new string[0];

		public override string this[string key] {
			get {
				return QueryString[key] ??
					Form[key] ??
					Cookies[key]?.Value ??
					ServerVariables[key];
			}
		}
		public override string[] AcceptTypes { get { return acceptTypes; } }
		public override string AnonymousID { get { return anonymousID; } }
		public override string ApplicationPath { get { return applicationPath; } }
		public override string AppRelativeCurrentExecutionFilePath {
			get { return appRelativeCurrentExecutionFilePath; }
		}
		public override HttpBrowserCapabilitiesBase Browser { get { return browser; } }
		public override HttpClientCertificate ClientCertificate { get { return clientCertificate; } }
		public override Encoding ContentEncoding {
			get { return contentEncoding; }
			set { contentEncoding = value; }
		}
		public override int ContentLength { get { return contentLength; } }
		public override string ContentType {
			get { return contentType; }
			set { contentType = value; }
		}
		public override HttpCookieCollection Cookies { get { return cookies; } }
		public override string CurrentExecutionFilePath { get { return currentExecutionFilePath; } }
		public override string CurrentExecutionFilePathExtension {
			get { return currentExecutionFilePathExtension; }
		}
		public override string FilePath { get { return filePath; } }
		public override HttpFileCollectionBase Files { get { return files; } }
		public override Stream Filter {
			get { return filter; }
			set { filter = value; }
		}
		public override NameValueCollection Form { get { return form; } }
		public override NameValueCollection Headers { get { return headers; } }
		public override ChannelBinding HttpChannelBinding { get { return httpChannelBinding; } }
		public override string HttpMethod { get { return httpMethod; } }
		public override Stream InputStream { get { return inputStream; } }
		public override bool IsAuthenticated { get { return isAuthenticated; } }
		public override bool IsLocal { get { return isLocal; } }
		public override bool IsSecureConnection { get { return isSecureConnection; } }
		public override WindowsIdentity LogonUserIdentity { get { return logonUserIdentity; } }
		public override NameValueCollection Params {
			get {
				if (@params == null) {
					@params = new NameValueCollection();
					@params.Add(QueryString);
					@params.Add(Form);
					Cookies.OfType<HttpCookie>().ForEach(c => @params[c.Name] = c.Value);
					@params.Add(ServerVariables);
				}
				return @params;
			}
		}
		public override string Path { get { return path; } }
		public override string PathInfo { get { return pathInfo; } }
		public override string PhysicalApplicationPath { get { return physicalPath; } }
		public override string PhysicalPath { get { return physicalPath; } }
		public override NameValueCollection QueryString { get { return queryString; } }
		public override string RawUrl { get { return rawUrl; } }
		public override ReadEntityBodyMode ReadEntityBodyMode { get { return readEntityBodyMode; } }
		public override RequestContext RequestContext {
			get { return requestContext; }
			set { requestContext = value; }
		}
		public override string RequestType {
			get { return requestType; }
			set { requestType = null; }
		}
		public override NameValueCollection ServerVariables { get { return serverVariables; } }
		public override CancellationToken TimedOutToken { get { return timedOutToken; } }
		public override int TotalBytes { get { return totalBytes; } }
		public override UnvalidatedRequestValuesBase Unvalidated { get { return unvalidated; } }
		public override Uri Url { get { return url; } }
		public override Uri UrlReferrer { get { return urlReferrer; } }
		public override string UserAgent { get { return userAgent; } }
		public override string UserHostAddress { get { return userHostAddress; } }
		public override string UserHostName { get { return userHostName; } }
		public override string[] UserLanguages { get { return userLanguages; } }
	}
}
