using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using DotLiquid;
using DryIoc;
using ZKWeb.Templating.AreaSupport;

namespace ZKWeb.Templating.TemplateTags {
	/// <summary>
	/// 用于提供动态内容的区域
	/// 区域Id要求全局唯一
	/// 例子
	/// {% area test_area %}
	/// 
	/// 描画流程
	/// - 读取自定义模块列表，存在时描画这个列表
	/// - 描画默认的模块列表
	/// 
	/// 生成的Html例子（使用[]代替）
	/// [div class='template_area' area_id='test_area']
	///		[div class='template_widget'][/div]
	///		[div class='template_widget'][/div]
	///		[div class='template_widget'][/div]
	/// [/div]
	/// </summary>
	public class Area : Tag {
		/// <summary>
		/// 保存当前区域Id时使用的键名
		/// </summary>
		public static string CurrentAreaIdKey { get; set; } = "__current_area_id";

		/// <summary>
		/// 描画标签
		/// </summary>
		/// <param name="context"></param>
		/// <param name="result"></param>
		public override void Render(Context context, TextWriter result) {
			var areaId = Markup.Trim();
			// 区域不能嵌套
			if (context[CurrentAreaIdKey] != null) {
				throw new FormatException("area tag can't be nested");
			}
			// 获取模块列表
			var areaManager = Application.Ioc.Resolve<TemplateAreaManager>();
			var widgets = areaManager.GetCustomWidgets(areaId) ??
				areaManager.GetArea(areaId).DefaultWidgets;
			// 添加div的开头
			result.Write($"<div class='template_area' area_id='{areaId}' >");
			// 描画子元素
			var scope = Hash.FromDictionary(new Dictionary<string, object>() {
				{ CurrentAreaIdKey, areaId }
			});
			context.Stack(scope, () => {
				foreach (var widget in widgets) {
					result.Write(areaManager.RenderWidget(context, widget));
				}
			});
			// 添加div的末尾
			result.Write("</div>");
		}
	}
}
