using ZKWebStandard.Web;

namespace ZKWeb.Web {
	/// <summary>
	/// 请求结果的接口
	/// </summary>
	public interface IActionResult {
		/// <summary>
		/// 把数据写到Http回应
		/// </summary>
		/// <param name="response">Http回应</param>
		void WriteResponse(IHttpResponse response);
	}
}
