using ZKWeb.Templating;
using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// Render template to response<br/>
	/// 描画模板到回应<br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	/// <example>
	/// <code language="cs">
	/// public ExampleController : IController {
	///		[Action("example")]
	///		public IActionResult Example() {
	///			return new TemplateResult("index.html", new { a = 123 });
	///		}
	///	}
	/// </code>
	/// </example>
	public class TemplateResult : IActionResult {
		/// <summary>
		/// Template path<br/>
		/// Eg: "test/test.html", "Common.Base:test/test.html"<br/>
		/// 模板路径<br/>
		/// 例如: "test/test.html", "Common.Base:test/test.html"<br/>
		/// </summary>
		public string TemplatePath { get; set; }
		/// <summary>
		/// Template arguments<br/>
		/// Can be anonymous object or IDictionary&lt;string, object&gt;<br/>
		/// 模板参数<br/>
		/// 可以是匿名对象或者IDictionary&lt;string, object&gt;<br/>
		/// </summary>
		public object TemplateArgument { get; set; }
		/// <summary>
		/// Content Type<br/>
		/// Default is "text/html; charset=utf-8"<br/>
		/// 内容类型<br/>
		/// 默认是 "text/html; charset=utf-8"<br/>
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="path">Template path</param>
		/// <param name="argument">Template arguments</param>
		public TemplateResult(string path, object argument = null) {
			TemplatePath = path;
			TemplateArgument = argument;
			ContentType = "text/html; charset=utf-8";
		}

		/// <summary>
		/// Render template to http response<br/>
		/// 描画模板到http回应<br/>
		/// </summary>
		/// <param name="response">Http response</param>
		public void WriteResponse(IHttpResponse response) {
			// Set status and mime
			response.StatusCode = 200;
			response.ContentType = ContentType;
			// Render template to http response
			var templateManager = Application.Ioc.Resolve<TemplateManager>();
			templateManager.RenderTemplate(TemplatePath, TemplateArgument, response.Body);
			response.Body.Flush();
		}
	}
}
