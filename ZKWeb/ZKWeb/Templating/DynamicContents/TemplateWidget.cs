using Newtonsoft.Json;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template widget
	/// Inside the template area, use to display dynamic contents
	/// </summary>
	public class TemplateWidget {
		/// <summary>
		/// Widget path
		/// </summary>
		public string Path { get; protected set; }
		/// <summary>
		/// Widget arguments
		/// It will open a scope let widget template use these variables
		/// eg: if arguments is new { a = 123 }, then {{ a }} will perform 123
		/// </summary>
		public object Args { get; protected set; }
		/// <summary>
		/// Serialize result of Args
		/// </summary>
		[JsonIgnore]
		protected string argsJson = null;
		/// <summary>
		/// Serialize result of Args, cached
		/// </summary>
		[JsonIgnore]
		public string ArgsJson {
			get {
				if (argsJson == null && Args != null) {
					argsJson = JsonConvert.SerializeObject(Args);
				}
				return argsJson;
			}
		}

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
