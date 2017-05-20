using DotLiquid;
using System.IO;

namespace ZKWeb.Templating.TemplateTags {
	/// <summary>
	/// Render text as raw html<br/>
	/// 把文本作为Html描画<br/>
	/// </summary>
	/// <seealso cref="TemplateManager"/>
	/// <example>
	/// <code>
	/// {% raw_html variable %}
	/// </code>
	/// </example>
	public class RawHtml : Tag {
		/// <summary>
		/// Render contents<br/>
		/// 描画内容<br/>
		/// </summary>
		public override void Render(Context context, TextWriter result) {
			result.Write(context[Markup.Trim()]);
		}
	}
}
