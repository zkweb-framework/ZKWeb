using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ZKWeb.Templating;
using ZKWeb.Web.Interfaces;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// 模板结果
	/// </summary>
	public class TemplateResult : IActionResult {
		/// <summary>
		/// 模板路径
		/// 这里的路径是虚拟路径
		/// 例如"test/test.html"或"Common.Base:test/test.html"
		/// </summary>
		public string TemplatePath { get; set; }
		/// <summary>
		/// 传给模板的参数
		/// 可以是匿名对象或IDictionary(string, object)
		/// </summary>
		public object TemplateArgument { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="path">模板路径，参考TemplatePath成员的注释</param>
		/// <param name="argument">传给模板的参数，参考TemplateArgument成员的注释</param>
		public TemplateResult(string path, object argument = null) {
			TemplatePath = path;
			TemplateArgument = argument;
		}

		/// <summary>
		/// 写入到http回应中
		/// </summary>
		/// <param name="response">http回应</param>
		public void WriteResponse(HttpResponseBase response) {
			var templateManager = Application.Ioc.Resolve<TemplateManager>();
			response.ContentType = "text/html";
			templateManager.RenderTemplate(TemplatePath, TemplateArgument, response.OutputStream);
		}
	}
}
