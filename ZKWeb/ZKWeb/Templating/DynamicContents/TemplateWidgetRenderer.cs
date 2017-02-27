using DotLiquid;
using DotLiquid.Tags;
using System.IO;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Built-in template widget renderer
	/// </summary>
	public class TemplateWidgetRenderer : ITemplateWidgetRenderer {
		/// <summary>
		/// Get html before widget contents
		/// </summary>
		protected virtual string GetBeforeHtml(Context context, TemplateWidget widget) {
			var cacheKey = widget.GetCacheKey();
			return $"<div class='template_widget' data-widget='{cacheKey}'>";
		}

		/// <summary>
		/// Get html after widget contents
		/// </summary>
		protected virtual string GetAfterHtml(Context context, TemplateWidget widget) {
			return "</div>";
		}

		/// <summary>
		/// Render widget
		/// Return render result
		/// </summary>
		public string Render(Context context, TemplateWidget widget) {
			var templateManager = Application.Ioc.Resolve<TemplateManager>();
			var writer = new StringWriter();
			var scope = templateManager.CreateHash(widget.Args);
			context.Stack(scope, () => {
				writer.Write(GetBeforeHtml(context, widget));
				var includeTag = new Include();
				var htmlPath = widget.Path + TemplateWidgetInfo.HtmlExtension;
				includeTag.Initialize("include", htmlPath, null);
				includeTag.Render(context, writer);
				writer.Write(GetAfterHtml(context, widget));
			});
			return writer.ToString();
		}
	}
}
