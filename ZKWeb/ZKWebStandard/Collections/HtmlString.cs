namespace ZKWebStandard.Collection {
	/// <summary>
	/// Html字符串
	/// 用于包装字符串
	/// </summary>
	public class HtmlString {
		/// <summary>
		/// 字符串值
		/// </summary>
		protected string Value { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="value">字符串值</param>
		public HtmlString(string value) {
			Value = value;
		}

		/// <summary>
		/// 返回字符串值
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return Value;
		}
	}
}
