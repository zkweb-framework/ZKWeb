using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// Redirect to other url<br/>
	/// 重定向到其他Url<br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	/// <example>
	/// <code language="cs">
	/// public ExampleController : IController {
	///		[Action("example")]
	///		public IActionResult Example() {
	///			return new RedirectResult("/login");
	///		}
	///	}
	/// </code>
	/// </example>
	public class RedirectResult : IActionResult {
		/// <summary>
		/// Redirect to url<br/>
		/// 重定向到的Url<br/>
		/// </summary>
		public string Url { get; set; }
		/// <summary>
		/// Is permanent, affect robot from search engine<br/>
		/// 是否永久重定向, 影响搜索引擎的蜘蛛<br/>
		/// </summary>
		public bool Permanent { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="url">Redirect to url</param>
		/// <param name="permanent">Is permanent</param>
		public RedirectResult(string url, bool permanent = false) {
			Url = url;
			Permanent = permanent;
		}

		/// <summary>
		/// Send redirect response<br/>
		/// 发送重定向回应<br/>
		/// </summary>
		/// <param name="response">Http response</param>
		public void WriteResponse(IHttpResponse response) {
			response.Redirect(Url, Permanent);
		}
	}
}
