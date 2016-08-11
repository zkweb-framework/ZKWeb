namespace ZKWeb.Localize {
	/// <summary>
	/// Interface for language
	/// </summary>
	public interface ILanguage {
		/// <summary>
		/// ISO language name，eg: zh-CN
		/// </summary>
		string Name { get; }
	}
}
