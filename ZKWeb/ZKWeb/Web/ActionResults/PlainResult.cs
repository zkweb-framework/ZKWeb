using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// Plain test result
	/// </summary>
	public class PlainResult : IActionResult {
		/// <summary>
		/// The text
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="obj">It will call ToString to get the text form this object</param>
		public PlainResult(object obj) {
			Text = obj?.ToString();
		}

		/// <summary>
		/// Write text to http response
		/// </summary>
		/// <param name="response">Http response</param>
		public void WriteResponse(IHttpResponse response) {
			// Set status and mime
			response.StatusCode = 200;
			response.ContentType = "text/plain";
			// Write text to http response
			response.Write(Text);
		}
	}
}
