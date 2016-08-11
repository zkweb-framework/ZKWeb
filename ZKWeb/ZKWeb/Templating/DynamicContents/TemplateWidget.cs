namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template widget
	/// Inside the template area, use to display dynamic contents
	/// </summary>
	public class TemplateWidget {
		/// <summary>
		/// Widget path
		/// </summary>
		public string Path { get; set; }
		/// <summary>
		/// Widget arguments
		/// It will open a scope let widget template use these variables
		/// eg: if arguments is new { a = 123 }, then {{ a }} will perform 123
		/// </summary>
		public object Args { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="path">Widget path, must without extension</param>
		/// <param name="args">Widget arguments</param>
		public TemplateWidget(string path, object args = null) {
			Path = path;
			Args = args;
		}
	}
}
