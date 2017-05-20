namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template widget extension methods<br/>
	/// 模板模块的扩展函数<br/>
	/// </summary>
	/// <seealso cref="TemplateWidget"/>
	public static class TemplateWidgetExtensions {
		/// <summary>
		/// Get cache key for template widget<br/>
		/// 获取模板模块的缓存键<br/>
		/// </summary>
		/// <param name="widget">Template widget</param>
		/// <returns></returns>
		public static string GetCacheKey(this TemplateWidget widget) {
			var cacheKey = widget.Path;
			var argsJson = widget.ArgsJson;
			if (argsJson != null) {
				cacheKey += argsJson;
			}
			return cacheKey;
		}
	}
}
