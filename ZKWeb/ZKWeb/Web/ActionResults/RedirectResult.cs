using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// Redirect result
	/// </summary>
	public class RedirectResult : IActionResult {
		/// <summary>
		/// Redirect to url
		/// </summary>
		public string Url { get; set; }
		/// <summary>
		/// Is permanent
		/// </summary>
		public bool Permanent { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="url">Redirect to url</param>
		/// <param name="permanent">Is permanent</param>
		public RedirectResult(string url, bool permanent = false) {
			Url = url;
			Permanent = permanent;
		}

		/// <summary>
		/// Send redirect response
		/// </summary>
		/// <param name="response">Http response</param>
		public void WriteResponse(IHttpResponse response) {
			response.Redirect(Url, Permanent);
		}
	}
}
