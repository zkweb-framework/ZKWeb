using ZKWeb.Templating;
using ZKWeb.Web;
using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// Template result
	/// </summary>
	public class TemplateResult : IActionResult {
		/// <summary>
		/// Template path
		/// Eg: "test/test.html", "Common.Base:test/test.html"
		/// </summary>
		public string TemplatePath { get; set; }
		/// <summary>
		/// Template arguments
		/// Can be anonymous object or IDictionary[string, object]
		/// </summary>
		public object TemplateArgument { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="path">Template path</param>
		/// <param name="argument">Template arguments</param>
		public TemplateResult(string path, object argument = null) {
			TemplatePath = path;
			TemplateArgument = argument;
		}

		/// <summary>
		/// Render template to http response
		/// </summary>
		/// <param name="response">Http response</param>
		public void WriteResponse(IHttpResponse response) {
			// Set status and mime
			response.StatusCode = 200;
			response.ContentType = "text/html";
			// Render template to http response
			var templateManager = Application.Ioc.Resolve<TemplateManager>();
			templateManager.RenderTemplate(TemplatePath, TemplateArgument, response.Body);
			response.Body.Flush();
		}
	}
}
