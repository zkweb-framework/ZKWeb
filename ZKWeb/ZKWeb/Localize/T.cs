using DotLiquid;

namespace ZKWeb.Localize {
	/// <summary>
	/// 翻译文本的帮助对象
	/// 实际翻译会在转换到字符串时执行，使用T类型保存文本可以用于延迟翻译
	/// 这个类可以直接转换到string，也可以使用ToString转换
	/// </summary>
	public struct T : ILiquidizable {
		/// <summary>
		/// 翻译前的文本
		/// </summary>
		private string Text { get; set; }

		/// <summary>
		/// 翻译文本
		/// </summary>
		/// <param name="text">文本</param>
		public T(string text) {
			Text = text;
		}

		/// <summary>
		/// 获取翻译后的文本
		/// </summary>
		/// <param name="t"></param>
		public static implicit operator string(T t) {
			return t.ToString();
		}

		/// <summary>
		/// 允许描画到模板
		/// </summary>
		/// <returns></returns>
		object ILiquidizable.ToLiquid() {
			return ToString();
		}

		/// <summary>
		/// 获取翻译后的文本
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			var translateManager = Application.Ioc.Resolve<TranslateManager>();
			return translateManager.Translate(Text);
		}
	}
}
