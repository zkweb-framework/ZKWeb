using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Localize;

namespace ZKWeb.Templating.TemplateFilters {
	/// <summary>
	/// 模板系统的过滤器
	/// </summary>
	public static class Filters {
		/// <summary>
		/// 翻译指定的文本
		/// 例子
		/// {{ text | trans }}
		/// {{ "fixed text" | trans }}
		/// </summary>
		/// <param name="text">需要翻译的文本</param>
		/// <returns></returns>
		public static string Trans(string text) {
			return new T(text);
		}

		/// <summary>
		/// 格式化字符串
		/// 最多可支持8个参数
		/// 例子
		/// {{ "name is [0], age is [1]" | format: name, age }}
		/// </summary>
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
		/// 把字符串作为原始html描画
		/// 例子
		/// {{ variable | raw_html }}
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static object RawHtml(string text) {
			return new HtmlString(text);
		}
	}
}
