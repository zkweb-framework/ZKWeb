using DotLiquid;
using DotLiquid.Tags;
using DryIoc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Templating.Diy;

namespace ZKWeb.Templating.TemplateTags {
	/// <summary>
	/// 模块
	/// 例子
	/// {% widget common.base.logo %}
	/// {% widget common.base.logo { w: 300, h: 200 } %}
	/// </summary>
	public class Widget : Tag {
		/// <summary>
		/// 描画内容
		/// </summary>
		/// <param name="context"></param>
		/// <param name="result"></param>
		public override void Render(Context context, TextWriter result) {
			// 获取所在区域，没有区域时抛例外
			if (context[Area.CurrentAreaIdKey] == null) {
				throw new FormatException("widget must use inside area");
			}
			// 获取模块名称和参数
			var markup = Markup.Trim();
			string widgetPath = null;
			string widgetArgs = null;
			var index = markup.IndexOf(' ');
			if (index > 0) {
				widgetPath = markup.Substring(0, index);
				widgetArgs = markup.Substring(index + 1);
			} else {
				widgetPath = markup;
			}
			// 描画模块的内容
			var diyManager = Application.Ioc.Resolve<DiyManager>();
			var renderResult = diyManager.GetWidgetRenderCache(widgetPath, widgetArgs);
			if (renderResult == null) {
				// 没有缓存，需要重新描画
				// 添加div的开头
				var writer = new StringWriter();
				writer.Write($"<div class='diy_widget' path='{widgetPath}' args='{widgetArgs}'>");
				// 重新描画模块内容
				var scope = widgetArgs == null ? new Hash() : Hash.FromDictionary(
					JsonConvert.DeserializeObject<IDictionary<string, object>>(widgetArgs));
				context.Stack(scope, () => {
					var includeTag = new Include();
					var htmlPath = widgetPath + DiyWidgetInfo.HtmlExtension;
					includeTag.Initialize("include", htmlPath, null);
					includeTag.Render(context, writer);
				});
				// 添加div的末尾
				writer.Write("</div>");
				// 保存到缓存中
				renderResult = writer.ToString();
				diyManager.PutWidgetRenderCache(widgetPath, widgetArgs, renderResult);
			}
			result.Write(renderResult);
		}
	}
}
