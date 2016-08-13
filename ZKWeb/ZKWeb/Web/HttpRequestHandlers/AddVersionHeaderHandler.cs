using ZKWebStandard.Web;

namespace ZKWeb.Web.HttpRequestHandlers {
	/// <summary>
	/// Add version header to http response
	/// </summary>
	public class AddVersionHeaderHandler : IHttpRequestPreHandler {
		/// <summary>
		/// Handle request
		/// </summary>
		public void OnRequest() {
			var response = HttpManager.CurrentContext.Response;
			response.AddHeader("X-ZKWeb-Version", Application.FullVersion);
		}
	}
}
