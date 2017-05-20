using DotLiquid;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template widget renderer<br/>
	/// 模板模块描画器<br/>
	/// </summary>
	/// <seealso cref="TemplateManager"/>
	public interface ITemplateWidgetRenderer {
		/// <summary>
		/// Render widget<br/>
		/// Return render result<br/>
		/// 描画模板, 返回描画结果<br/>
		/// </summary>
		/// <param name="context">Template context</param>
		/// <param name="widget">Template widget</param>
		string Render(Context context, TemplateWidget widget);
	}
}
