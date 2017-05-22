using ZKWebStandard.Utils;

namespace ZKWebStandard.Collection {
	/// <summary>
	/// Html string wrapper<br/>
	/// <br/>
	/// </summary>
	public class HtmlString {
		/// <summary>
		/// Html string<br/>
		/// <br/>
		/// </summary>
		protected string Value { get; set; }

		/// <summary>
		/// Initialize with html string, no encoding occurred<br/>
		/// <br/>
		/// </summary>
		/// <param name="value">Html string</param>
		public HtmlString(string value) {
			Value = value;
		}

		/// <summary>
		/// Return html string<br/>
		/// <br/>
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return Value;
		}

		/// <summary>
		/// Decode html string as text<br/>
		/// <br/>
		/// </summary>
		/// <returns></returns>
		public string Decode() {
			return HttpUtils.HtmlDecode(Value);
		}

		/// <summary>
		/// Encode text as html string<br/>
		/// <br/>
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static HtmlString Encode(string text) {
			return new HtmlString(HttpUtils.HtmlEncode(text));
		}
	}
}
