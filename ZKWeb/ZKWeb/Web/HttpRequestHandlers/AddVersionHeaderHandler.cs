using ZKWebStandard.Web;

namespace ZKWeb.Web.HttpRequestHandlers {
	/// <summary>
	/// Add zkweb version header to http response<br/>
	/// 添加ZKWeb版本标识到Http回应<br/>
	/// </summary>
	/// <seealso cref="IHttpRequestPreHandler"/>
	public class AddVersionHeaderHandler : IHttpRequestPreHandler {
		/// <summary>
		/// Handle request<br/>
		/// 处理请求<br/>
		/// </summary>
		public void OnRequest() {
			var response = HttpManager.CurrentContext.Response;
			response.AddHeader("X-ZKWeb-Version", Application.FullVersion);
		}
	}
}
