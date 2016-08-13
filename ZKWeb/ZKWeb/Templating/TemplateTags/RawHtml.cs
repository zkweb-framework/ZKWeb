using DotLiquid;
using System.IO;

namespace ZKWeb.Templating.TemplateTags {
	/// <summary>
	/// Render text as raw html
	/// </summary>
	/// <example>{% raw_html variable %}</example>
	public class RawHtml : Tag {
		/// <summary>
		/// Render contents
		/// </summary>
		public override void Render(Context context, TextWriter result) {
			result.Write(context[Markup.Trim()]);
		}
	}
}
