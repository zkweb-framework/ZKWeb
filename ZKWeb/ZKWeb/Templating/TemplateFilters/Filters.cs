using ZKWeb.Localize;
using ZKWebStandard.Collection;

namespace ZKWeb.Templating.TemplateFilters {
	/// <summary>
	/// Dotliquid template filters
	/// Dotliquid模板使用的过滤器<br/>
	/// </summary>
	/// <seealso cref="TemplateManager"/>
	public static class Filters {
		/// <summary>
		/// Translate text<br/>
		/// 翻译文本<br/>
		/// </summary>
		/// <example>
		/// <code>
		/// {{ text | trans }}
		/// {{ "fixed text" | trans }}
		/// </code>
		/// </example>
		/// <param name="text">Original text</param>
		/// <returns></returns>
		public static string Trans(string text) {
			return new T(text);
		}

		/// <summary>
		/// Format string<br/>
		/// Support up to 8 parameters<br/>
		/// 格式化支付穿<br/>
		/// 最多支持8个参数<br/>
		/// </summary>
		/// <example>
		/// <code>
		/// {{ "name is [0], age is [1]" | format: name, age }}
		/// </code>
		/// </example>
		/// <returns></returns>
		public static string Format(string text,
			object arg_0 = null, object arg_1 = null, object arg_2 = null, object arg_3 = null,
			object arg_4 = null, object arg_5 = null, object arg_6 = null, object arg_7 = null) {
			text = text.Replace("[0]", arg_0?.ToString());
			text = text.Replace("[1]", arg_1?.ToString());
			text = text.Replace("[2]", arg_2?.ToString());
			text = text.Replace("[3]", arg_3?.ToString());
			text = text.Replace("[4]", arg_4?.ToString());
			text = text.Replace("[5]", arg_5?.ToString());
			text = text.Replace("[6]", arg_6?.ToString());
			text = text.Replace("[7]", arg_7?.ToString());
			return text;
		}

		/// <summary>
		/// Render text as raw html<br/>
		/// 把文本作为html描画<br/>
		/// </summary>
		/// <example>
		/// <code>
		/// {{ variable | raw_html }}
		/// </code>
		/// </example>
		/// <param name="text">Html text</param>
		/// <returns></returns>
		public static object RawHtml(string text) {
			return new HtmlString(text);
		}
	}
}
