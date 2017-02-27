using DotLiquid;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template widget renderer
	/// </summary>
	public interface ITemplateWidgetRenderer {
		/// <summary>
		/// Render widget
		/// Return render result
		/// </summary>
		/// <param name="context">Template context</param>
		/// <param name="widget">Template widget</param>
		string Render(Context context, TemplateWidget widget);
	}
}
