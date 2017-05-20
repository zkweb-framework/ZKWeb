using System.Collections.Generic;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template widget information extension methods<br/>
	/// 模板模块的信息的扩展函数<br/>
	/// </summary>
	/// <seealso cref="TemplateWidgetInfo"/>
	public static class TemplateWidgetInfoExtensions {
		/// <summary>
		/// Get cache isolation policy names<br/>
		/// With "Device" anyway even it's not in the configuration<br/>
		/// 获取缓存隔离策略的名称列表<br/>
		/// 会添加"Device"无论它是否在配置中<br/>
		/// </summary>
		/// <param name="info">Widget information</param>
		/// <returns></returns>
		public static IList<string> GetCacheIsolationPolicyNames(this TemplateWidgetInfo info) {
			var result = new List<string>();
			if (!string.IsNullOrEmpty(info.CacheBy)) {
				result.AddRange(info.CacheBy.Split(','));
			}
			result.Add("Device");
			return result;
		}
	}
}
