using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// Plain test result<br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	public class PlainResult : IActionResult {
		/// <summary>
		/// The text<br/>
		/// <br/>
		/// </summary>
		public string Text { get; set; }
		/// <summary>
		/// Content Type<br/>
		/// Default is "text/plain; charset=utf-8"<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		/// <param name="obj">It will call ToString to get the text form this object</param>
		public PlainResult(object obj) {
			Text = obj?.ToString();
			ContentType = "text/plain; charset=utf-8";
		}

		/// <summary>
		/// Write text to http response<br/>
		/// <br/>
		/// </summary>
		/// <param name="response">Http response</param>
		public void WriteResponse(IHttpResponse response) {
			// Set status and mime
			response.StatusCode = 200;
			response.ContentType = ContentType;
			// Write text to http response
			response.Write(Text);
		}
	}
}
