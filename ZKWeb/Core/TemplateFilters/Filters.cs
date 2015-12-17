using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKWeb.Core.TemplateFilters {
	/// <summary>
	/// 模板系统的过滤器
	/// </summary>
	public static class Filters {
		/// <summary>
		/// 翻译指定的文本
		/// 例子
		/// {% text | trans %}
		/// {% "fixed text" | trans %}
		/// </summary>
		/// <param name="text">需要翻译的文本</param>
		/// <returns></returns>
		public static string Trans(string text) {
			return new T(text);
		}
	}
}
