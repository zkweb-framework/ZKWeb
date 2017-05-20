using DotLiquid;
using DotLiquid.Tags;
using System;
using System.IO;
using ZKWebStandard.Extensions;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Built-in template widget renderer<br/>
	/// 内置的模板模块描画器<br/>
	/// </summary>
	/// <seealso cref="TemplateWidget"/>
	/// <seealso cref="TemplateAreaManager"/>
	public class TemplateWidgetRenderer : ITemplateWidgetRenderer {
		/// <summary>
		/// Get html before widget contents<br/>
		/// 获取描画前的Html内容<br/>
		/// </summary>
		protected virtual string GetBeforeHtml(Context context, TemplateWidget widget) {
			var cacheKey = widget.GetCacheKey();
			return $"<div class='template_widget' data-widget='{cacheKey}'>";
		}

		/// <summary>
		/// Get html after widget contents<br/>
		/// 获取描画后的Html内容<br/>
		/// </summary>
		protected virtual string GetAfterHtml(Context context, TemplateWidget widget) {
			return "</div>";
		}

		/// <summary>
		/// Render widget<br/>
		/// Return render result<br/>
		/// 描画模板模块<br/>
		/// 返回描画结果<br/>
		/// </summary>
		public string Render(Context context, TemplateWidget widget) {
			var templateManager = Application.Ioc.Resolve<TemplateManager>();
			var writer = new StringWriter();
			var scope = templateManager.CreateHash(widget.Args);
			context.Stack(scope, () => {
				writer.Write(GetBeforeHtml(context, widget));
				try {
					var includeTag = new Include();
					var htmlPath = widget.Path + TemplateWidgetInfo.HtmlExtension;
					includeTag.Initialize("include", htmlPath, null);
					includeTag.Render(context, writer);
				} catch (Exception ex) {
					writer.Write(ex.ToDetailedString());
				}
				writer.Write(GetAfterHtml(context, widget));
			});
			return writer.ToString();
		}
	}
}
