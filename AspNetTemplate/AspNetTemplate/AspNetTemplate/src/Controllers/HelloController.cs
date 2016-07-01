using ZKWeb.Web;
using ZKWeb.Web.ActionResults;
using ZKWebStandard.Ioc;

namespace AspNetTemplate.AspNetTemplate.src.Controllers {
	/// <summary>
	/// Hello world controller
	/// </summary>
	[ExportMany]
	public class HelloController : IController {
		/// <summary>
		/// GET /hello
		/// </summary>
		/// <returns></returns>
		[Action("hello")]
		public IActionResult Hello() {
			return new TemplateResult("aspnettemplate/hello.html", new { text = "World" });
		}
	}
}
