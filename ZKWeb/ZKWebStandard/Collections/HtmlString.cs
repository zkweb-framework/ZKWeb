namespace ZKWebStandard.Collection {
	/// <summary>
	/// Html string wrapper
	/// </summary>
	public class HtmlString {
		/// <summary>
		/// Html string
		/// </summary>
		protected string Value { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="value">Html string</param>
		public HtmlString(string value) {
			Value = value;
		}

		/// <summary>
		/// Return html string
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return Value;
		}
	}
}
