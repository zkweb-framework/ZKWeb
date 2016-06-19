using DotLiquid;
using System.IO;

namespace ZKWeb.Templating.TemplateTags {
	/// <summary>
	/// 描画原始的内容，不经过html编码
	/// 例
	/// {% raw_html variable %}
	/// </summary>
	public class RawHtml : Tag {
		/// <summary>
		/// 描画内容
		/// </summary>
		/// <param name="context"></param>
		/// <param name="result"></param>
		public override void Render(Context context, TextWriter result) {
			result.Write(context[Markup.Trim()]);
		}
	}
}
