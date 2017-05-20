using System.Collections.Generic;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template area<br/>
	/// 模板区域<br/>
	/// </summary>
	/// <seealso cref="TemplateWidget"/>
	/// <seealso cref="TemplateWidgetInfo"/>
	/// <seealso cref="TemplateAreaManager"/>
	public class TemplateArea {
		/// <summary>
		/// Area Id<br/>
		/// 区域Id<br/>
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// Default widgets<br/>
		/// 该区域下的默认模块列表<br/>
		/// </summary>
		public IList<TemplateWidget> DefaultWidgets { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="id">Area id</param>
		public TemplateArea(string id) {
			Id = id;
			DefaultWidgets = new List<TemplateWidget>();
		}
	}
}
