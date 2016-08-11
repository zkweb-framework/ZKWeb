using System.Collections.Generic;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template widget information extension methods
	/// </summary>
	public static class TemplateWidgetInfoExtensions {
		/// <summary>
		/// Get cache isolation policy names
		/// With "Device" anyway even it's not in the configuration
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
