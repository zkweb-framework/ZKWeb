using System;
using System.Collections.Generic;
using System.IO;
using DotLiquid;
using ZKWeb.Templating.DynamicContents;

namespace ZKWeb.Templating.TemplateTags {
	/// <summary>
	/// Dotliquid area tag
	/// Id must be unique in all templates
	/// Flow
	/// - Render custom widgets if it's specified, otherwise
	/// - Render default widgets
	/// </summary>
	/// <example>
	/// {% area test_area %}
	/// Generated html
	/// [div class='template_area' area_id='test_area']
	///		[div class='template_widget'][/div]
	///		[div class='template_widget'][/div]
	///		[div class='template_widget'][/div]
	/// [/div]
	/// </example>
	public class Area : Tag {
		/// <summary>
		/// Key name use to store area id
		/// Use to detect nested area
		/// </summary>
		public static string CurrentAreaIdKey { get; set; } = "__current_area_id";
		/// <summary>
		/// Area id
		/// </summary>
		public string AreaId { get; protected set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public override void Initialize(string tagName, string markup, List<string> tokens) {
			// Call base method
			base.Initialize(tagName, markup, tokens);
			// Get area id
			AreaId = Markup.Trim();
		}

		/// <summary>
		/// Render tag
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
