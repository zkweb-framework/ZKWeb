namespace ZKWeb.Localize {
	/// <summary>
	/// Interface for translate text
	/// </summary>
	public interface ITranslateProvider {
		/// <summary>
		/// Determine this provider can translate text to the given language
		/// </summary>
		bool CanTranslate(string code);

		/// <summary>
		/// Translate text
		/// It should return null if it can't translate the text
		/// </summary>
		/// <param name="text">Original text</param>
		/// <returns></returns>
		string Translate(string text);
	}
}
