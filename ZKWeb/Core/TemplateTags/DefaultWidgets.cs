using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace ZKWeb.Core.TemplateTags {
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
			var areaId = context[Area.CurrentAreaIdKey];
			result.WriteLine($"default widgets of area {areaId}");
		}
	}
}
