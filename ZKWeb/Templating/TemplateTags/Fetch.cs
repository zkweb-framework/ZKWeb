using DotLiquid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ZKWebStandard.Extensions;
using ZKWeb.Web.ActionResults;
using ZKWeb.Web;
using ZKWebStandard.Web;
using ZKWebStandard.Web.Mock;
using ZKWebStandard.Utils;
using ZKWebStandard.Web.Wrappers;
using ZKWebStandard.Collections;

namespace ZKWeb.Templating.TemplateTags {
	/// <summary>
	/// 把指定路径的执行结果设置到变量
	/// 指定的路径可以是get也可以是post，会自动检测
	/// 路径中的变量的获取顺序
	/// - 等于"*query"时使用当前请求的参数
	/// - 当前模板上下文中的变量
	/// - 当前http上下文中的参数
	/// 执行结果
	/// - 结果是JsonResult或PlainResult时使用返回的结果
	/// - 其他结果时使用描画的内容，但不支持二进制
	/// </summary>
	/// <example>
	/// {% fetch /api/example_info > example_info %}
	/// {{ example_info }}
	///	url支持变量
	/// {% fetch /api/example_info?id={id} > example_info %}
	/// {% fetch /api/example_info?{*query} > example_info %}
	/// {{ example_info }}
	/// </example>
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
		/// 路径和请求参数
		/// </summary>
		public string PathAndQuery { get; protected set; }
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
			PathAndQuery = Markup.Substring(0, index).Trim();
			Variable = Markup.Substring(index + 1).Trim();
		}

		/// <summary>
		/// 把指定url的执行结果设置到变量
		/// </summary>
		public override void Render(Context context, TextWriter result) {
			// 设置路径中的变量
			var pathAndQuery = PathAndQuery;
			var matches = VariableInUrlExpression.Matches(pathAndQuery);
			foreach (Match match in matches) {
				var name = match.Value.Substring(1, match.Value.Length - 2);
				string value = null;
				if (name == VariableNameForAllQueryParameters) {
					value = HttpManager.CurrentContext.Request.QueryString;
					if (value.Length > 0 && value[0] == '?') {
						value = value.Substring(1);
					}
				} else if ((value = context[name].ConvertOrDefault<string>()) != null) {
				} else {
					value = HttpManager.CurrentContext.Request.Get<string>(name);
				}
				pathAndQuery = pathAndQuery.Replace(match.Value, value);
			}
			// 查找对应的处理函数
			var controllerManager = Application.Ioc.Resolve<ControllerManager>();
			string path;
			string queryString;
			HttpUtils.SplitPathAndQuery(pathAndQuery, out path, out queryString);
			var method = HttpMethods.POST;
			var action = controllerManager.GetAction(path, method);
			if (action == null) {
				method = HttpMethods.GET;
				action = controllerManager.GetAction(path, method);
			}
			if (action == null) {
				throw new KeyNotFoundException($"action {path} not found");
			}
			// 执行处理函数
			// 使用模拟的Http上下文，继承Items, Cookies, Header
			var fetchContext = new FetchHttpContext(path, queryString, method);
			using (HttpManager.OverrideContext(fetchContext)) {
				var actionResult = action();
				if (actionResult is PlainResult) {
					context[Variable] = ((PlainResult)actionResult).Text;
				} else if (actionResult is JsonResult) {
					context[Variable] = ((JsonResult)actionResult).Object;
				} else {
					var response = fetchContext.FetchResponse;
					actionResult.WriteResponse(response);
					context[Variable] = response.GetContentsFromBody();
				}
			}
		}

		/// <summary>
		/// 抓取数据时使用的Http上下文
		/// 继承当前上下文的Items
		/// </summary>
		internal class FetchHttpContext : IHttpContext {
			public IHttpContext ParentContext { get; set; }
			public FetchHttpRequest FetchRequest { get; set; }
			public HttpResponseMock FetchResponse { get; set; }

			public IDictionary<object, object> Items { get { return ParentContext.Items; } }
			public IHttpRequest Request { get { return FetchRequest; } }
			public IHttpResponse Response { get { return FetchResponse; } }

			public FetchHttpContext(string path, string queryString, string method) {
				ParentContext = HttpManager.CurrentContext;
				FetchRequest = new FetchHttpRequest(this, path, queryString, method);
				FetchResponse = new HttpResponseMock(this);
			}
		};

		/// <summary>
		/// 抓取数据时使用的Http请求
		/// 继承当前上下文的Cookies和Headers
		/// </summary>
		internal class FetchHttpRequest : HttpRequestWrapper {
			public FetchHttpContext FetchContext { get; set; }
			public string FetchPath { get; set; }
			public string FetchQueryString { get; set; }
			public string FetchMethod { get; set; }
			public IDictionary<string, IList<string>> FetchQuery { get; set; }
			public IDictionary<string, IList<string>> FetchForm { get; set; }

			public override Stream Body { get { throw new NotSupportedException(); } }
			public override long? ContentLength { get { throw new NotSupportedException(); } }
			public override IHttpContext HttpContext { get { return FetchContext; } }
			public override string Path { get { return FetchPath; } }
			public override string QueryString { get { return FetchQueryString; } }
			public override string Method { get { return FetchMethod; } }
			public override IList<string> GetQueryValue(string key) {
				return FetchQuery.GetOrDefault(key);
			}
			public override IEnumerable<Pair<string, IList<string>>> GetQueryValues() {
				foreach (var pair in FetchQuery) {
					yield return Pair.Create(pair.Key, pair.Value);
				}
			}
			public override IList<string> GetFormValue(string key) {
				return FetchForm.GetOrDefault(key);
			}
			public override IEnumerable<Pair<string, IList<string>>> GetFormValues() {
				foreach (var pair in FetchForm) {
					yield return Pair.Create(pair.Key, pair.Value);
				}
			}
			public override IHttpPostedFile GetPostedFile(string key) {
				return null;
			}
			public override IEnumerable<Pair<string, IHttpPostedFile>> GetPostedFiles() {
				yield break;
			}

			public FetchHttpRequest(
				FetchHttpContext fetchContext,
				string path, string queryString, string method) :
				base(HttpManager.CurrentContext.Request) {
				FetchContext = fetchContext;
				FetchPath = path;
				FetchMethod = method;
				FetchQueryString = queryString;
				FetchQuery = HttpUtils.ParseQueryString(queryString);
				FetchForm = FetchQuery;
			}
		};
	}
}
