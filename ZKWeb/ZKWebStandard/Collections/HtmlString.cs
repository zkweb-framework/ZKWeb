using ZKWebStandard.Utils;

namespace ZKWebStandard.Collection {
	/// <summary>
	/// Html string wrapper<br/>
	/// Html字符串的包装类<br/>
	/// </summary>
	/// <example>
	/// <code language="cs">
	/// var html = new HtmlString("&lt;div&gt;&lt;/div&gt;");
	/// var rendered = (html is HtmlString) ? html.ToString() : HttpUtils.HtmlEncode(html);
	/// </code>
	/// </example>
	public class HtmlString {
		/// <summary>
		/// Html string<br/>
		/// Html字符串<br/>
		/// </summary>
		protected string Value { get; set; }

		/// <summary>
		/// Initialize with html string, no encoding occurred<br/>
		/// 以Html字符串初始化, 不进行编码<br/>
		/// </summary>
		/// <param name="value">Html string</param>
		public HtmlString(string value) {
			Value = value;
		}

		/// <summary>
		/// Return html string<br/>
		/// 返回Html字符串<br/>
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return Value;
		}

		/// <summary>
		/// Decode html string as text<br/>
		/// 解码Html字符串到字符串<br/>
		/// </summary>
		/// <returns></returns>
		public string Decode() {
			return HttpUtils.HtmlDecode(Value);
		}

		/// <summary>
		/// Encode text as html string<br/>
		/// 编码字符串到Html字符串<br/>
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static HtmlString Encode(string text) {
			return new HtmlString(HttpUtils.HtmlEncode(text));
		}
	}
}
