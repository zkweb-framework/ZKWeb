using ZKWebStandard.Extensions;
using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// Write plain test to response<br/>
	/// 写入文本到回应<br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	/// <example>
	/// <code language="cs">
	/// public ExampleController : IController {
	///		[Action("example")]
	///		public IActionResult Example() {
	///			return new PlainResult("abc");
	///		}
	///	}
	/// </code>
	/// </example>
	public class PlainResult : IActionResult {
		/// <summary>
		/// The text<br/>
		/// 文本<br/>
		/// </summary>
		public string Text { get; set; }
		/// <summary>
		/// Content Type<br/>
		/// Default is "text/plain; charset=utf-8"<br/>
		/// 内容类型<br/>
		/// 默认是"text/plain; charset=utf-8"<br/>
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="obj">It will call ToString to get the text form this object</param>
		public PlainResult(object obj) {
			Text = obj?.ToString();
			ContentType = "text/plain; charset=utf-8";
		}

		/// <summary>
		/// Write text to http response<br/>
		/// 写入文本到http回应<br/>
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
