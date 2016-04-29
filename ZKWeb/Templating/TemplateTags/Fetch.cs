using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using ZKWeb.Utils.Functions;
using System.Text.RegularExpressions;
using ZKWeb.Utils.Extensions;
using DryIoc;
using ZKWeb.Web.ActionResults;
using ZKWeb.Web;
using ZKWeb.Web.Interfaces;

namespace ZKWeb.Templating.TemplateTags {
	/// <summary>
	/// 把指定路径的执行结果设置到变量
	/// 指定的路径可以是get也可以是post，会自动检测
	/// 例子
	///		{% fetch /api/login_info > login_info %}
	///		{{ login_info }}
	///	例子，url支持变量
	///		{% fetch /api/product_info?id={id} > product_info %}
	///		{{ product_info }}
	///	路径中的变量的获取顺序
	///		当前模板上下文中的变量
	///		当前http上下文中的参数
	///	执行结果
	///		结果是JsonResult或PlainResult时使用返回的结果
	///		其他结果时使用描画的内容，但不支持二进制
	/// </summary>
	public class Fetch : Tag {
		/// <summary>
		/// 获取url中的变量使用的正则表达式
		/// </summary>
		public readonly static Regex VariableInUrlExpression = new Regex("{.+?}");

		/// <summary>
		/// 把指定url的执行结果设置到变量
		/// </summary>
		public override void Render(Context context, TextWriter result) {
			// 获取路径和变量
			var index = Markup.LastIndexOf('>');
			if (index <= 0 || index + 1 == Markup.Length) {
				throw new FormatException("incorrect format, please use {% fetch path > variable %}");
			}
			var path = Markup.Substring(0, index).Trim();
			var variable = Markup.Substring(index + 1).Trim();
			// 设置路径中的变量
			var matches = VariableInUrlExpression.Matches(path);
			foreach (Match match in matches) {
				var name = match.Value.Substring(1, match.Value.Length - 2);
				var value = context[name].ConvertOrDefault<string>();
				if (value == null) {
					value = HttpContext.Current.Request.GetParam<string>(name);
				}
				path = path.Replace(match.Value, value);
			}
			// 查找对应的处理函数
			var controllerManager = Application.Ioc.Resolve<ControllerManager>();
			var method = HttpMethods.GET;
			var action = controllerManager.GetAction(path, method);
			if (action == null) {
				method = HttpMethods.POST;
				action = controllerManager.GetAction(path, method);
			}
			if (action == null) {
				throw new KeyNotFoundException($"action {path} not found");
			}
			// 执行处理函数
			using (HttpContextUtils.UseContext(path, method)) {
				var actionResult = action();
				if (actionResult is PlainResult) {
					context[variable] = ((PlainResult)actionResult).Text;
				} else if (actionResult is JsonResult) {
					context[variable] = ((JsonResult)actionResult).Object;
				} else {
					var writer = new StringWriter();
					var response = new HttpResponse(writer);
					actionResult.WriteResponse(response);
					context[variable] = writer.ToString();
				}
			}
		}
	}
}
