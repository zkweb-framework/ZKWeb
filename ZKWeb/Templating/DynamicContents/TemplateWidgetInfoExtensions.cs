using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// 模板模块的信息的扩展函数
	/// </summary>
	public static class TemplateWidgetInfoExtensions {
		/// <summary>
		/// 获取缓存隔离策略的名称列表
		/// 默认会添加按设备隔离，即使模块信息中不指定
		/// </summary>
		/// <param name="info">模板模块信息</param>
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
