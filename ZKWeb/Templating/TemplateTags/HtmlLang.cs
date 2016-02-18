using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Threading;

namespace ZKWeb.Templating.TemplateTags {
	/// <summary>
	/// 显示当前的页面语言代号
	/// 例子
	///		html lang="{% html_lang %}"
	/// </summary>
	public class HtmlLang : Tag {
		/// <summary>
		/// 描画内容
		/// </summary>
		/// <param name="context"></param>
		/// <param name="result"></param>
		public override void Render(Context context, TextWriter result) {
			result.Write(Thread.CurrentThread.CurrentCulture.Name);
		}
	}
}
