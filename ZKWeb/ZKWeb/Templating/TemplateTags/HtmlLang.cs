using DotLiquid;
using System.Globalization;
using System.IO;

namespace ZKWeb.Templating.TemplateTags {
	/// <summary>
	/// Display the language code<br/>
	/// 显示当前语言代码<br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="TemplateManager"/>
	/// <example>
	/// <code>
	/// &lt;html lang="{% html_lang %}"&gt;&lt;/html&gt;
	/// </code>
	/// </example>
	public class HtmlLang : Tag {
		/// <summary>
		/// Render contents<br/>
		/// 描画内容<br/>
		/// </summary>
		public override void Render(Context context, TextWriter result) {
			result.Write(CultureInfo.CurrentCulture.Name);
		}
	}
}
