using System.Collections.Generic;
using ZKWebStandard.Extensions;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template area extension methods
	/// </summary>
	public static class TemplateAreaExtensions {
		/// <summary>
		/// Add widget
		/// </summary>
		/// <param name="widgets">Widgets</param>
		/// <param name="path">Widget path, must without extension</param>
		/// <param name="args">Arguments</param>
		public static void Add(
			this IList<TemplateWidget> widgets, string path, object args = null) {
			widgets.Add(new TemplateWidget(path, args));
		}

		/// <summary>
		/// Add widget before the specified widget
		/// If specified widget not found then add it to the front
		/// </summary>
		/// <param name="widgets">Widgets</param>
		/// <param name="beforePath">Add before widget that path equals it</param>
		/// <param name="path">Widget path, must without extension</param>
		/// <param name="args">Arguments</param>
		public static void AddBefore(
			this IList<TemplateWidget> widgets, string beforePath, string path, object args = null) {
			widgets.AddBefore(x => x.Path == beforePath, new TemplateWidget(path, args));
		}

		/// <summary>
		/// Add widget after the specified widget
		/// If specified widget not found then add it to the back
		/// </summary>
		/// <param name="widgets">Widgets</param>
		/// <param name="afterPath">Add after widget that path equals it</param>
		/// <param name="path">Widget path, must without extension</param>
		/// <param name="args">Arguments</param>
		public static void AddAfter(
			this IList<TemplateWidget> widgets, string afterPath, string path, object args = null) {
			widgets.AddAfter(x => x.Path == afterPath, new TemplateWidget(path, args));
		}
	}
}
