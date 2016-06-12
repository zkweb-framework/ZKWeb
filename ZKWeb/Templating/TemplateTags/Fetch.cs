using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using ZKWeb.Utils.Functions;
using System.Text.RegularExpressions;
using ZKWeb.Utils.Extensions;
using ZKWeb.Web.ActionResults;
using ZKWeb.Web;
using ZKWeb.Web.Interfaces;
using ZKWeb.Utils.Collections;

namespace ZKWeb.Templating.TemplateTags {
	/// <summary>
	/// 把指定路径的执行结果设置到变量
	/// 指定的路径可以是get也可以是post，会自动检测
	/// <example>
	/// {% fetch /api/example_info > example_info %}
	/// {{ example_info }}
	///	url支持变量
	/// {% fetch /api/example_info?id={id} > example_info %}
	/// {% fetch /api/example_info?{*query} > example_info %}
	/// {{ example_info }}
	/// </example>
	/// 路径中的变量的获取顺序
	/// - 等于"*query"时使用当前请求的参数
	/// - 当前模板上下文中的变量
	/// - 当前http上下文中的参数
	/// 执行结果
	/// - 结果是JsonResult或PlainResult时使用返回的结果
	/// - 其他结果时使用描画的内容，但不支持二进制
	/// </summary>
	public class Fetch : Tag {
		/// <summary>
		/// 获取url中的变量使用的正则表达式
		/// </summary>
		public readonly static Regex VariableInUrlExpression = new Regex("{.+?}");
		/// <summary>
		/// 使用当前请求的参数的变量名
		/// </summary>
		public const string VariableNameForAllQueryParameters = "*query";
		/// <summary>
		/// 路径
		/// </summary>
		public string Path { get; protected set; }
		/// <summary>
		/// 变量
		/// </summary>
		public string Variable { get; protected set; }

		/// <summary>
		/// 初始化
		/// </summary>
		public override void Initialize(string tagName, string markup, List<string> tokens) {
			// 调用基础类处理
			base.Initialize(tagName, markup, tokens);
			// 获取路径和变量
			var index = Markup.LastIndexOf('>');
			if (index <= 0 || index + 1 == Markup.Length) {
				throw new FormatException("incorrect format, please use {% fetch path > variable %}");
			}
			Path = Markup.Substring(0, index).Trim();
			Variable = Markup.Substring(index + 1).Trim();
		}

		/// <summary>
		/// 把指定url的执行结果设置到变量
		/// </summary>
		public override void Render(Context context, TextWriter result) {
			// 设置路径中的变量
			var path = Path;
			var matches = VariableInUrlExpression.Matches(path);
			foreach (Match match in matches) {
				var name = match.Value.Substring(1, match.Value.Length - 2);
				string value = null;
				if (name == VariableNameForAllQueryParameters) {
					value = HttpContextUtils.CurrentContext.Request.Url.Query;
					if (value.Length > 0 && value[0] == '?') {
						value = value.Substring(1);
					}
				} else if ((value = context[name].ConvertOrDefault<string>()) != null) {
				} else {
					value = HttpContextUtils.CurrentContext.Request.Get<string>(name);
				}
				path = path.Replace(match.Value, value);
			}
			// 查找对应的处理函数
			var controllerManager = Application.Ioc.Resolve<ControllerManager>();
			var queryIndex = path.IndexOf('?');
			var pathWithoutQuery = (queryIndex >= 0) ? path.Substring(0, queryIndex) : path;
			var method = HttpMethods.GET;
			var action = controllerManager.GetAction(pathWithoutQuery, method);
			if (action == null) {
				method = HttpMethods.POST;
				action = controllerManager.GetAction(pathWithoutQuery, method);
			}
			if (action == null) {
				throw new KeyNotFoundException($"action {pathWithoutQuery} not found");
			}
			// 执行处理函数
			using (HttpContextUtils.OverrideContext(path, method)) {
				var actionResult = action();
				if (actionResult is PlainResult) {
					context[Variable] = ((PlainResult)actionResult).Text;
				} else if (actionResult is JsonResult) {
					context[Variable] = ((JsonResult)actionResult).Object;
				} else {
					var response = new HttpResponseMock();
					response.outputStream = new MemoryStream();
					actionResult.WriteResponse(response);
					response.outputStream.Seek(0, SeekOrigin.Begin);
					context[Variable] = new StreamReader(response.outputStream).ReadToEnd();
				}
			}
		}
	}
}
