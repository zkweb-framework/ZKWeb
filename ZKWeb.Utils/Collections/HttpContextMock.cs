using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Instrumentation;
using System.Web.Profile;

#pragma warning disable 1591
namespace ZKWeb.Utils.Collections {
	/// <summary>
	/// http上下文的模拟类
	/// </summary>
	public class HttpContextMock : HttpContextBase {
		public Exception[] allErrors = new Exception[0];
		public bool allowAsyncDuringSyncStages = false;
		public HttpApplicationStateBase application = null;
		public HttpApplication applicationInstance = null;
		public AsyncPreloadModeFlags asyncPreloadMode = AsyncPreloadModeFlags.None;
		public Cache cache = new Cache();
		public IHttpHandler currentHandler = null;
		public RequestNotification currentNotification = RequestNotification.BeginRequest;
		public Exception error = null;
		public IHttpHandler handler = null;
		public bool isCustomErrorEnabled = true;
		public bool isDebuggingEnabled = true;
		public bool isPostNotification = false;
		public bool isWebSocketRequest = false;
		public bool isWebSocketRequestUpgrading = false;
		public IDictionary items = new Dictionary<string, object>();
		public PageInstrumentationService pageInstrumentation = null;
		public IHttpHandler previousHandler = null;
		public ProfileBase profile = null;
		public HttpRequestBase request = null;
		public HttpResponseBase response = null;
		public HttpServerUtilityBase server = null;
		public HttpSessionStateBase session = null;
		public bool skipAuthorization = false;
		public bool threadAbortOnTimeout = false;
		public DateTime timestamp = DateTime.UtcNow;
		public TraceContext trace = null;
		public IPrincipal user = null;
		public string webSocketNegotiatedProtocol = null;
		public IList<string> webSocketRequestedProtocols = new string[0];

		public override Exception[] AllErrors { get { return allErrors; } }
		public override bool AllowAsyncDuringSyncStages {
			get { return allowAsyncDuringSyncStages; }
			set { allowAsyncDuringSyncStages = value; }
		}
		public override HttpApplicationStateBase Application { get { return application; } }
		public override HttpApplication ApplicationInstance {
			get { return applicationInstance; }
			set { applicationInstance = value; }
		}
		public override AsyncPreloadModeFlags AsyncPreloadMode {
			get { return asyncPreloadMode; }
			set { asyncPreloadMode = value; }
		}
		public override Cache Cache { get { return cache; } }
		public override IHttpHandler CurrentHandler { get { return currentHandler; } }
		public override RequestNotification CurrentNotification { get { return currentNotification; } }
		public override Exception Error { get { return error; } }
		public override IHttpHandler Handler {
			get { return handler; }
			set { handler = value; }
		}
		public override bool IsCustomErrorEnabled { get { return isCustomErrorEnabled; } }
		public override bool IsDebuggingEnabled { get { return isDebuggingEnabled; } }
		public override bool IsPostNotification { get { return isPostNotification; } }
		public override bool IsWebSocketRequest { get { return isWebSocketRequest; } }
		public override bool IsWebSocketRequestUpgrading { get { return isWebSocketRequestUpgrading; } }
		public override IDictionary Items { get { return items; } }
		public override PageInstrumentationService PageInstrumentation { get { return pageInstrumentation; } }
		public override IHttpHandler PreviousHandler { get { return previousHandler; } }
		public override ProfileBase Profile { get { return profile; } }
		public override HttpRequestBase Request { get { return request; } }
		public override HttpResponseBase Response { get { return response; } }
		public override HttpServerUtilityBase Server { get { return server; } }
		public override HttpSessionStateBase Session { get { return session; } }
		public override bool SkipAuthorization {
			get { return skipAuthorization; }
			set { skipAuthorization = value; }
		}
		public override bool ThreadAbortOnTimeout {
			get { return threadAbortOnTimeout; }
			set { threadAbortOnTimeout = value; }
		}
		public override DateTime Timestamp { get { return timestamp; } }
		public override TraceContext Trace { get { return trace; } }
		public override IPrincipal User {
			get { return user; }
			set { user = value; }
		}
		public override string WebSocketNegotiatedProtocol { get { return webSocketNegotiatedProtocol; } }
		public override IList<string> WebSocketRequestedProtocols { get { return webSocketRequestedProtocols; } }
	}
}
#pragma warning restore 1591
