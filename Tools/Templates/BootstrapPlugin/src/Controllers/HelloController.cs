using ZKWeb;
using ZKWeb.Storage;
using ZKWeb.Web;
using ZKWeb.Web.ActionResults;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ${ProjectName}.Plugins.${ProjectName}.src.Controllers {
	/// <summary>
	/// Example controller only for learning, delete it if you don't need
	/// </summary>
	public class HelloController : IController {
		[Action("/")]
		public IActionResult Index() {
			return new TemplateResult("${ProjectNameLower}/hello.html", new { text = "World" });
		}

		[Action("/hello")]
		public IActionResult Hello() {
			return new PlainResult("Hello World In Plain Text");
		}
	}

	/// <summary>
	/// Static file handler, delete it if you are already use default plugin collections
	/// </summary>
	public class HelloStaticHandler : IHttpRequestHandler {
		public const string Prefix = "/static/";

		public void OnRequest() {
			var context = HttpManager.CurrentContext;
			var path = context.Request.Path;
			if (path.StartsWith(Prefix)) {
				var fileStorage = Application.Ioc.Resolve<IFileStorage>();
				var subPath = HttpUtils.UrlDecode(path.Substring(Prefix.Length));
				var fileEntry = fileStorage.GetResourceFile("static", subPath);
				if (fileEntry.Exists) {
					var ifModifiedSince = context.Request.GetIfModifiedSince();
					new FileEntryResult(fileEntry, ifModifiedSince).WriteResponse(context.Response);
					context.Response.End();
				}
			}
		}
	}
}
