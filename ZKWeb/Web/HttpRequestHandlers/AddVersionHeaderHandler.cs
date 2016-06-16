using ZKWeb.Web;
using ZKWebStandard.Web;

namespace ZKWeb.Web.HttpRequestHandlers {
	/// <summary>
	/// 添加版本头信息到回应
	/// </summary>
	public class AddVersionHeaderHandler : IHttpRequestPreHandler {
		/// <summary>
		/// 处理请求
		/// </summary>
		public void OnRequest() {
			var response = HttpManager.CurrentContext.Response;
			response.AddHeader("X-ZKWeb-Version", Application.FullVersion);
		}
	}
}
