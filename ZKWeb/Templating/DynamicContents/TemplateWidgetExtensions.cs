using Newtonsoft.Json;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// 模板模块的扩展函数
	/// </summary>
	public static class TemplateWidgetExtensions {
		/// <summary>
		/// 获取模板模块对应的缓存键
		/// </summary>
		/// <param name="widget">模板模块</param>
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
