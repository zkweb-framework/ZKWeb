using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// Redirect result<br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	public class RedirectResult : IActionResult {
		/// <summary>
		/// Redirect to url<br/>
		/// <br/>
		/// </summary>
		public string Url { get; set; }
		/// <summary>
		/// Is permanent<br/>
		/// <br/>
		/// </summary>
		public bool Permanent { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
		/// </summary>
		/// <param name="url">Redirect to url</param>
		/// <param name="permanent">Is permanent</param>
		public RedirectResult(string url, bool permanent = false) {
			Url = url;
			Permanent = permanent;
		}

		/// <summary>
		/// Send redirect response<br/>
		/// <br/>
		/// </summary>
		/// <param name="response">Http response</param>
		public void WriteResponse(IHttpResponse response) {
			response.Redirect(Url, Permanent);
		}
	}
}
