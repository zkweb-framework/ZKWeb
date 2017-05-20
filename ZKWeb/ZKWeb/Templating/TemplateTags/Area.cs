using System;
using System.Collections.Generic;
using System.IO;
using DotLiquid;
using ZKWeb.Templating.DynamicContents;

namespace ZKWeb.Templating.TemplateTags {
	/// <summary>
	/// Dotliquid area tag<br/>
	/// Id must be unique in all templates<br/>
	/// Flow<br/>
	/// - Render custom widgets if it's specified, otherwise<br/>
	/// - Render default widgets<br/>
	/// 区域标签<br/>
	/// Id必须在所有模板中唯一<br/>
	/// 流程<br/>
	/// - 如果有定义自定义模块则描画自定义模块, 否则<br/>
	/// - 描画默认模块<br/>
	/// </summary>
	/// <seealso cref="TemplateManager"/>
	/// <example>
	/// <code>
	/// {% area test_area %}
	/// </code>
	/// 
	/// <code>
	/// Generates html
	/// &lt;div class='template_area' area_id='test_area'&gt;
	///		&lt;div class='template_widget'&gt;&lt;/div&gt;
	///		&lt;div class='template_widget'&gt;&lt;/div&gt;
	///		&lt;div class='template_widget'&gt;&lt;/div&gt;
	/// &lt;/div&gt;
	/// </code>
	/// </example>
	public class Area : Tag {
		/// <summary>
		/// Key name use to store area id<br/>
		/// Use to detect nested area<br/>
		/// 储存区域Id的键名<br/>
		/// 用于检测区域是否嵌套<br/>
		/// </summary>
		public static string CurrentAreaIdKey { get; set; } = "__current_area_id";
		/// <summary>
		/// Area id<br/>
		/// 区域Id<br/>
		/// </summary>
		public string AreaId { get; protected set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public override void Initialize(string tagName, string markup, List<string> tokens) {
			// Call base method
			base.Initialize(tagName, markup, tokens);
			// Get area id
			AreaId = Markup.Trim();
		}

		/// <summary>
		/// Render tag<br/>
		/// 描画标签<br/>
		/// </summary>
		/// <param name="context"></param>
		/// <param name="result"></param>
		public override void Render(Context context, TextWriter result) {
			// Nested area is unsupported
			if (context[CurrentAreaIdKey] != null) {
				throw new FormatException("area tag can't be nested");
			}
			// Get child widgets
			var areaManager = Application.Ioc.Resolve<TemplateAreaManager>();
			var widgets = areaManager.GetCustomWidgets(AreaId) ??
				areaManager.GetArea(AreaId).DefaultWidgets;
			// Render div begin tag
			result.Write($"<div class='template_area' area_id='{AreaId}'>");
			// Render child widgets
			var scope = new Hash();
			scope.Add(CurrentAreaIdKey, AreaId);
			context.Stack(scope, () => {
				foreach (var widget in widgets) {
					result.Write(areaManager.RenderWidget(context, widget));
				}
			});
			// Render div end tag
			result.Write("</div>");
		}
	}
}
