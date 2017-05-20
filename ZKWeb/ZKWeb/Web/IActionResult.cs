using ZKWebStandard.Web;

namespace ZKWeb.Web {
	/// <summary>
	/// Interface of action result<br/>
	/// Action结果的接口<br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	/// <example>
	/// <code language="cs">
	/// public class PlainResult : IActionResult {
	///		public string Text { get; set; }
	///		public string ContentType { get; set; }
	///
	///		public PlainResult(object obj) {
	///			Text = obj?.ToString();
	///			ContentType = "text/plain; charset=utf-8";
	///		}
	///
	///		public void WriteResponse(IHttpResponse response) {
	///			response.StatusCode = 200;
	///			response.ContentType = ContentType;
	///			response.Write(Text);
	///		}
	/// }
	/// </code>
	/// </example>
	public interface IActionResult {
		/// <summary>
		/// Write http response<br/>
		/// 写入到http回应<br/>
		/// </summary>
		/// <param name="response">Http response</param>
		void WriteResponse(IHttpResponse response);
	}
}
