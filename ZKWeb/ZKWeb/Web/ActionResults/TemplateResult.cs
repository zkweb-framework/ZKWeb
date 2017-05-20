using ZKWeb.Templating;
using ZKWeb.Web;
using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// Template result<br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	public class TemplateResult : IActionResult {
		/// <summary>
		/// Template path<br/>
		/// Eg: "test/test.html", "Common.Base:test/test.html"<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		public string TemplatePath { get; set; }
		/// <summary>
		/// Template arguments<br/>
		/// Can be anonymous object or IDictionary[string, object]<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		public object TemplateArgument { get; set; }
		/// <summary>
		/// Content Type<br/>
		/// Default is "text/html; charset=utf-8"<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// <br/>
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
		/// <br/>
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
