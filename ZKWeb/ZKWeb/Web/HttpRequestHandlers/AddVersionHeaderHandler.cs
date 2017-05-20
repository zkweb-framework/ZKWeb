using ZKWebStandard.Web;

namespace ZKWeb.Web.HttpRequestHandlers {
	/// <summary>
	/// Add version header to http response<br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="IHttpRequestPreHandler"/>
	public class AddVersionHeaderHandler : IHttpRequestPreHandler {
		/// <summary>
		/// Handle request<br/>
		/// <br/>
		/// </summary>
		public void OnRequest() {
			var response = HttpManager.CurrentContext.Response;
			response.AddHeader("X-ZKWeb-Version", Application.FullVersion);
		}
	}
}
