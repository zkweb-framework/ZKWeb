namespace ZKWeb.Localize {
	/// <summary>
	/// Interface for providing text translation<br/>
	/// 用于提供文本翻译的接口<br/>
	/// </summary>
	/// <seealso cref="TranslateManager"/>
	/// <seealso cref="T"/>
	/// <example>
	/// <code>
	/// [ExportMany, SingletonReuse]
	///	public class zh_CN : ITranslateProvider {
	///		private static HashSet&lt;string&gt; Codes = new HashSet&lt;string&gt;() { "zh-CN" };
	///		private static Dictionary&lt;string, string&gt; Translates = new Dictionary&lt;string, string&gt;()
	///		{
	///			{ "Example", "示例" }
	///		};
	///
	///		public bool CanTranslate(string code) {
	///			return Codes.Contains(code);
	///		}
	///
	///		public string Translate(string text) {
	///			return Translates.GetOrDefault(text);
	///		}
	///	}
	/// </code>
	/// </example>
	public interface ITranslateProvider {
		/// <summary>
		/// Determine this provider can translate text to the given language<br/>
		/// 判断这个提供器是否可以翻译文本到指定的语言<br/>
		/// </summary>
		/// <param name="code">Language code</param>
		/// <returns></returns>
		bool CanTranslate(string code);

		/// <summary>
		/// Translate text<br/>
		/// It should return null if it can't translate the text<br/>
		/// 翻译文本<br/>
		/// 在不能翻译该文本时应该返回null<br/>
		/// </summary>
		/// <param name="text">Original text</param>
		/// <returns></returns>
		string Translate(string text);
	}
}
