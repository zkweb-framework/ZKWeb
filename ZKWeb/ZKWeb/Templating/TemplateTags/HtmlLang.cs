using DotLiquid;
using System.Globalization;
using System.IO;

namespace ZKWeb.Templating.TemplateTags {
	/// <summary>
	/// Display the language name
	/// </summary>
	/// <example>
	/// html lang="{% html_lang %}"
	/// </example>
	public class HtmlLang : Tag {
		/// <summary>
		/// Render contents
		/// </summary>
		public override void Render(Context context, TextWriter result) {
			result.Write(CultureInfo.CurrentCulture.Name);
		}
	}
}
