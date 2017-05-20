namespace ZKWeb.Localize {
	/// <summary>
	/// Interface for language<br/>
	/// 语言的接口<br/>
	/// </summary>
	/// <seealso cref="TranslateManager"/>
	/// <example>
	/// <code language="cs">
	/// [ExportMany]
	/// public class Chinese : ILanguage {
	///		public string Name { get { return "zh-CN"; } }
	/// }
	/// </code>
	/// </example>
	public interface ILanguage {
		/// <summary>
		/// ISO language name，eg: zh-CN<br/>
		/// ISO语言名称, 例如: zh-CN<br/>
		/// </summary>
		string Name { get; }
	}
}
