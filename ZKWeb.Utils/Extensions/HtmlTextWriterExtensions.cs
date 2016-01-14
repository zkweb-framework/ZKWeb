using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace ZKWeb.Utils.Extensions {
	/// <summary>
	/// Html构建器的扩展函数
	/// </summary>
	public static class HtmlTextWriterExtensions {
		/// <summary>
		/// 批量添加元素的属性
		/// </summary>
		/// <param name="html">html构建器</param>
		/// <param name="attributes">元素属性的集合</param>
		public static void AddAttributes(
			this HtmlTextWriter html, IEnumerable<KeyValuePair<string, string>> attributes) {
			foreach (var attr in attributes) {
				html.AddAttribute(attr.Key, attr.Value);
			}
		}
	}
}
