using System.Collections.Generic;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template area
	/// </summary>
	public class TemplateArea {
		/// <summary>
		/// Area Id
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// Default widgets
		/// </summary>
		public IList<TemplateWidget> DefaultWidgets { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="id">Area id</param>
		public TemplateArea(string id) {
			Id = id;
			DefaultWidgets = new List<TemplateWidget>();
		}
	}
}
