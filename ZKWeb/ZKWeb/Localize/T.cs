using DotLiquid;

namespace ZKWeb.Localize {
	/// <summary>
	/// A class wrap original text and return translated text<br/>
	/// Translation will perform when calling ToString<br/>
	/// This class can convert to string implicit, or use ToString explicit<br/>
	/// 用于包装原文并返回译文的类<br/>
	/// 翻译会在调用ToString函数时执行<br/>
	/// 这个类可以隐式转换到string, 也可以显式调用ToString函数<br/>
	/// </summary>
	/// <seealso cref="ITranslateProvider"/>
	/// <seealso cref="TranslateManager"/>
	/// <example>
	/// <code language="cs">
	/// string translated = new T("Original");
	/// </code>
	/// </example>
	public struct T : ILiquidizable {
		/// <summary>
		/// Original text<br/>
		/// 原文<br/>
		/// </summary>
		private string Text { get; set; }
		/// <summary>
		/// Format parameters<br/>
		/// 格式化参数<br/>
		/// </summary>
		private object[] Parameters { get; set; }

		/// <summary>
		/// Get the translation of text<br/>
		/// 获取文本的翻译<br/>
		/// </summary>
		/// <param name="text">Original text</param>
		public T(string text) : this(text, null) { }

		/// <summary>
		/// Get the translation of formatted text<br/>
		/// 获取格式化的文本的翻译<br/>
		/// </summary>
		/// <param name="text">Original text</param>
		/// <param name="parameters">Format parameters</param>
		public T(string text, params object[] parameters) {
			Text = text;
			Parameters = parameters;
		}

		/// <summary>
		/// Get translated text<br/>
		/// 获取翻译后的文本<br/>
		/// </summary>
		/// <param name="t">This object</param>
		public static implicit operator string(T t) {
			return t.ToString();
		}

		/// <summary>
		/// Support render to template<br/>
		/// 支持描画到模板<br/>
		/// </summary>
		/// <returns></returns>
		object ILiquidizable.ToLiquid() {
			return ToString();
		}

		/// <summary>
		/// Get translated text<br/>
		/// 获取翻译后的文本<br/>
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			var translateManager = Application.Ioc.Resolve<TranslateManager>();
			var text = translateManager.Translate(Text);
			if (Parameters != null) {
				text = string.Format(text, Parameters);
			}
			return text;
		}
	}
}
