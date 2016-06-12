using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using DotLiquid;
using ZKWeb.Templating.AreaSupport;

namespace ZKWeb.Templating.TemplateTags {
	/// <summary>
	/// 用于提供动态内容的区域
	/// 区域Id要求全局唯一
	/// <example>
	/// {% area test_area %}
	/// </example>
	/// 描画流程
	/// - 读取自定义模块列表，存在时描画这个列表
	/// - 描画默认的模块列表
	/// <example>
	/// 生成的Html（使用[]代替）
	/// [div class='template_area' area_id='test_area']
	///		[div class='template_widget'][/div]
	///		[div class='template_widget'][/div]
	///		[div class='template_widget'][/div]
	/// [/div]
	/// </example>
	/// </summary>
	public class Area : Tag {
		/// <summary>
		/// 保存当前区域Id时使用的键名
		/// </summary>
		public static string CurrentAreaIdKey { get; set; } = "__current_area_id";
		/// <summary>
		/// 区域Id
		/// </summary>
		public string AreaId { get; protected set; }

		/// <summary>
		/// 初始化
		/// </summary>
		public override void Initialize(string tagName, string markup, List<string> tokens) {
			// 调用基础类的基础
			base.Initialize(tagName, markup, tokens);
			// 获取区域Id
			AreaId = Markup.Trim();
		}

		/// <summary>
		/// 描画标签
		/// </summary>
		/// <param name="context"></param>
		/// <param name="result"></param>
		public override void Render(Context context, TextWriter result) {
			// 区域不能嵌套
			if (context[CurrentAreaIdKey] != null) {
				throw new FormatException("area tag can't be nested");
			}
			// 获取模块列表
			var areaManager = Application.Ioc.Resolve<TemplateAreaManager>();
			var widgets = areaManager.GetCustomWidgets(AreaId) ??
				areaManager.GetArea(AreaId).DefaultWidgets;
			// 添加div的开头
			result.Write($"<div class='template_area' area_id='{AreaId}'>");
			// 描画子元素
			var scope = Hash.FromDictionary(new Dictionary<string, object>() {
				{ CurrentAreaIdKey, AreaId }
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
