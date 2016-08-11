using Newtonsoft.Json;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template widget extension methods
	/// </summary>
	public static class TemplateWidgetExtensions {
		/// <summary>
		/// Get cache key for template widget
		/// </summary>
		/// <param name="widget">Template widget</param>
		/// <returns></returns>
		public static string GetCacheKey(this TemplateWidget widget) {
			var cacheKey = widget.Path;
			if (widget.Args != null) {
				cacheKey += JsonConvert.SerializeObject(widget.Args);
			}
			return cacheKey;
		}
	}
}
