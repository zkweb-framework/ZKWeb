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
	/// Set template variable from action execute result<br/>
	/// Action can be get or post, will automatic detect<br/>
	/// Url can contains variables, variable can be<br/>
	/// - "*query": all query arguments from the visiting url<br/>
	/// - key: variable from template context<br/>
	/// - key: argument from visiting url<br/>
	/// If the action return JsonResult or PlainResult, the result will be the returned object,<br/>
	/// Otherwise it will be the rendered text of the result (binary data is unsupported)<br/>
	/// 根据Action结果设置模板变量<br/>
	/// Action可以是GET或POST, 会自动检测<br/>
	/// Url可以包含变量, 变量可以是<br/>
	/// - "*query": 当前请求的所有查询参数<br/>
	/// - 当前模板中的其他变量<br/>
	/// - 当前请求中的参数<br/>
	/// 如果Action返回JsonResult或者PlainResult, 则结果会是返回的对象<br/>
	/// 否则结果会是描画出来的文本 (不支持二进制数据)<br/>
	/// </summary>
	/// <seealso cref="TemplateManager"/>
	/// <example>
	/// Url<br/>
	/// <code>
	/// {% fetch /api/example_info > example_info %}
	/// {{ example_info }}
	/// </code>
	/// 
	///	Url with variable<br/>
	///	带参数的Url<br/>
	///	<code>
	/// {% fetch /api/example_info?id={id} > example_info %}
	/// {% fetch /api/example_info?{*query} > example_info %}
	/// {{ example_info }}
	/// </code>
	/// </example>
	public class Fetch : Tag {
		/// <summary>
		/// Regex for getting variable in the url<br/>
		/// 获取Url中的变量使用的正则表达式<br/>
		/// </summary>
		public readonly static Regex VariableInUrlExpression = new Regex("{.+?}");
		/// <summary>
		/// The variable name that mean all query arguments from the visiting url<br/>
		/// 用于表示当前请求中所有参数的变量名<br/>
		/// </summary>
		public const string VariableNameForAllQueryParameters = "*query";
		/// <summary>
		/// Path and query<br/>
		/// 路径和查询参数<br/>
		/// </summary>
		public string PathAndQuery { get; protected set; }
		/// <summary>
		/// Target template variable<br/>
		/// 保存到的变量名称<br/>
		/// </summary>
		public string Variable { get; protected set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public override void Initialize(string tagName, string markup, List<string> tokens) {
			// Call base method
			base.Initialize(tagName, markup, tokens);
			// Parse markup
			var index = Markup.LastIndexOf('>');
			if (index <= 0 || index + 1 == Markup.Length) {
				throw new FormatException("incorrect format, please use {% fetch path > variable %}");
			}
			PathAndQuery = Markup.Substring(0, index).Trim();
			Variable = Markup.Substring(index + 1).Trim();
		}

		/// <summary>
		/// Set template variable from action execute result<br/>
		/// 从Action的执行结果设置模板变量<br/>
		/// </summary>
		public override void Render(Context context, TextWriter result) {
			// Replace variables in url
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
			// Find the target action from controllers
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
			// Execute action
			// Use simulate http context, inhert Items, Cookies, Headers
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
		/// Simulated http context<br/>
		/// 模拟的Http上下文<br/>
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
		/// Simulated http request<br/>
		/// 模拟的Http请求<br/>
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
				if (method == HttpMethods.GET) {
					FetchQueryString = queryString;
					FetchQuery = HttpUtils.ParseQueryString(queryString);
					FetchForm = new Dictionary<string, IList<string>>();
				} else {
					FetchQueryString = "";
					FetchQuery = new Dictionary<string, IList<string>>();
					FetchForm = HttpUtils.ParseQueryString(queryString);
				}
			}
		};
	}
}
