using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKWeb.Model.ActionResults {
	/// <summary>
	/// 模板结果
	/// </summary>
	public class TemplateResult : IActionResult {
		/// <summary>
		/// 模板文件路径
		/// </summary>
		public string TemplatePath { get; set; }
		/// <summary>
		/// 传给模板的参数
		/// </summary>
		public object TemplateArgument { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="path">模板文件路径</param>
		/// <param name="argument">传给模板的参数</param>
		public TemplateResult(string path, object argument) {
			TemplatePath = path;
			TemplateArgument = argument;
		}

		/// <summary>
		/// 写入到http回应
		/// </summary>
		/// <param name="response"></param>
		public void WriteResponse(HttpResponse response) {
			throw new NotImplementedException();
		}
	}
}