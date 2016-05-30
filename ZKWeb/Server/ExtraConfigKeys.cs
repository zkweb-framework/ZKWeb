using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKWeb.Server {
	/// <summary>
	/// 网站附加配置中使用的键值
	/// 前缀需要添加"ZKWeb."
	/// </summary>
	internal static class ExtraConfigKeys {
		/// <summary>
		/// 翻译的缓存时间，单位是秒
		/// </summary>
		public const string TranslateCacheTime = "ZKWeb.TranslateCacheTime";
		/// <summary>
		/// 模板路径缓存时间，单位是秒
		/// </summary>
		public const string TemplatePathCacheTime = "ZKWeb.TemplatePathCacheTime";
		/// <summary>
		/// 资源路径的缓存时间，单位是秒
		/// </summary>
		public const string ResourcePathCacheTime = "ZKWeb.ResourcePathCacheTime";
		/// <summary>
		/// 模块信息的缓存时间，单位是秒
		/// </summary>
		public const string WidgetInfoCacheTime = "ZKWeb.WidgetInfoCacheTime";
		/// <summary>
		/// 自定义模块列表的缓存时间，单位是秒
		/// </summary>
		public const string CustomWidgetsCacheTime = "ZKWeb.CustomWidgetsCacheTime";
		/// <summary>
		/// 模板的缓存时间，单位是秒
		/// </summary>
		public const string TemplateCacheTime = "ZKWeb.TemplateCacheTime";
		/// <summary>
		/// 是否在描画模板发生例外时显示完整信息
		/// </summary>
		public const string DisplayFullExceptionForTemplate = "ZKWeb.DisplayFullExceptionForTemplate";
		/// <summary>
		/// 内存占用超过此数值时自动清理缓存，单位是MB
		/// </summary>
		public const string ClearCacheAfterUsedMemoryMoreThan = "ZKWeb.ClearCacheAfterUsedMemoryMoreThan";
		/// <summary>
		/// 缓存自动清理器的检查间隔，单位是秒
		/// </summary>
		public const string CleanCacheCheckInterval = "ZKWeb.CleanCacheCheckInterval";
	}
}
