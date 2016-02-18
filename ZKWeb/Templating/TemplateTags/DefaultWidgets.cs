using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using DryIoc;
using Newtonsoft.Json;
using ZKWeb.Templating.Diy;

namespace ZKWeb.Templating.TemplateTags {
	/// <summary>
	/// 区域中的默认模块
	/// 例子
	/// 内容可以在插件中动态注册，见DiyManager
	/// {% area test_area %}
	///     {% default_widgets %}
	/// {% endarea %}
	/// </summary>
	public class DefaultWidgets : Tag {
		/// <summary>
		/// 描画内容
		/// </summary>
		/// <param name="context"></param>
		/// <param name="result"></param>
		public override void Render(Context context, TextWriter result) {
			// 获取默认模块列表
			var areaId = context[Area.CurrentAreaIdKey]?.ToString();
			if (string.IsNullOrEmpty(areaId)) {
				throw new FormatException("default_widgets must use inside area");
			}
			var diyManager = Application.Ioc.Resolve<DiyManager>();
			var area = diyManager.GetArea(areaId);
			// 描画模块列表
			foreach (var widget in area.DefaultWidgets) {
				var widgetTag = new Widget();
				var markup = widget.Info.WidgetPath;
				if (widget.Args != null) {
					markup += " " + JsonConvert.SerializeObject(widget.Args);
				}
				widgetTag.Initialize("widget", markup, null);
				widgetTag.Render(context, result);
			}
		}
	}
}
