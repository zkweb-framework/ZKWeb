using ZKWebStandard.Web;

namespace ZKWeb.Web {
	/// <summary>
	/// Interface for action result<br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	public interface IActionResult {
		/// <summary>
		/// Write http response<br/>
		/// <br/>
		/// </summary>
		/// <param name="response">Http response</param>
		void WriteResponse(IHttpResponse response);
	}
}
