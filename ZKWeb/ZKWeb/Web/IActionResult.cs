using ZKWebStandard.Web;

namespace ZKWeb.Web {
	/// <summary>
	/// Interface for action result
	/// </summary>
	public interface IActionResult {
		/// <summary>
		/// Write http response
		/// </summary>
		/// <param name="response">Http response</param>
		void WriteResponse(IHttpResponse response);
	}
}
