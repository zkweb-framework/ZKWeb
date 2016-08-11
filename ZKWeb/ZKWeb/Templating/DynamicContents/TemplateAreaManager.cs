using DotLiquid;
using DotLiquid.Tags;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using ZKWeb.Cache;
using ZKWeb.Server;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template area and widgets manager
	/// </summary>
	public class TemplateAreaManager : ICacheCleaner {
		/// <summary>
		/// Widget information cache time
		/// Default is 2s, able to override from website configuration
		/// </summary>
		public TimeSpan WidgetInfoCacheTime { get; set; }
		/// <summary>
		/// Custom widgets cache time
		/// Default is 2s, able to override from website configuration
		/// </summary>
		public TimeSpan CustomWidgetsCacheTime { get; set; }
		/// <summary>
		/// Areas
		/// { Area id: Area }
		/// </summary>
		protected Dictionary<string, TemplateArea> Areas { get; set; }
		/// <summary>
		/// Widget information cache
		/// Isolated by client device
		/// { Widget path: widget information }
		/// </summary>
		protected IsolatedMemoryCache<string, TemplateWidgetInfo> WidgetInfoCache { get; set; }
		/// <summary>
		/// Custom widgets cache
		/// Isolated by client device
		/// { Area id: custom widgets }
		/// </summary>
		protected IsolatedMemoryCache<string, List<TemplateWidget>> CustomWidgetsCache { get; set; }
		/// <summary>
		/// Widget render result cache
		/// { Isolation policy: { Width path and arguments: render result } }
		/// </summary>
		protected ConcurrentDictionary<string,
			IsolatedMemoryCache<string, string>> WidgetRenderCache { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public TemplateAreaManager() {
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			WidgetInfoCacheTime = TimeSpan.FromSeconds(
				configManager.WebsiteConfig.Extra.GetOrDefault(ExtraConfigKeys.WidgetInfoCacheTime, 2));
			CustomWidgetsCacheTime = TimeSpan.FromSeconds(
				configManager.WebsiteConfig.Extra.GetOrDefault(ExtraConfigKeys.CustomWidgetsCacheTime, 2));
			Areas = new Dictionary<string, TemplateArea>();
			WidgetInfoCache = new IsolatedMemoryCache<string, TemplateWidgetInfo>("Device");
			CustomWidgetsCache = new IsolatedMemoryCache<string, List<TemplateWidget>>("Device");
			WidgetRenderCache = new ConcurrentDictionary<string, IsolatedMemoryCache<string, string>>();
		}

		/// <summary>
		/// Get area by id
		/// If not found then create a new area with the given id
		/// </summary>
		/// <param name="areaId">Area id</param>
		/// <returns></returns>
		public virtual TemplateArea GetArea(string areaId) {
			return Areas.GetOrCreate(areaId, () => new TemplateArea(areaId));
		}

		/// <summary>
		/// Get widget information
		/// </summary>
		/// <param name="widgetPath">Widget path</param>
		/// <returns></returns>
		public virtual TemplateWidgetInfo GetWidgetInfo(string widgetPath) {
			var info = WidgetInfoCache.GetOrDefault(widgetPath);
			if (info == null) {
				info = TemplateWidgetInfo.FromPath(widgetPath);
				WidgetInfoCache.Put(widgetPath, info, WidgetInfoCacheTime);
			}
			return info;
		}

		/// <summary>
		/// Get json path that store custom widgets for the given area
		/// Path: App_Data/areas/{areaId}.widgets
		/// </summary>
		/// <param name="areaId">Area id</param>
		/// <returns></returns>
		protected virtual string GetCustomWidgetsJsonPath(string areaId) {
			var pathManager = Application.Ioc.Resolve<PathManager>();
			var path = pathManager.GetStorageFullPath("areas", string.Format("{0}.widgets", areaId));
			return path;
		}

		/// <summary>
		/// Get custom widgets for the given area
		/// Return null if custom widgets are unspecified
		/// </summary>
		/// <param name="areaId">Area id</param>
		/// <returns></returns>
		public virtual List<TemplateWidget> GetCustomWidgets(string areaId) {
			return CustomWidgetsCache.GetOrCreate(areaId, () => {
				var path = GetCustomWidgetsJsonPath(areaId);
				if (File.Exists(path)) {
					return JsonConvert.DeserializeObject<List<TemplateWidget>>(File.ReadAllText(path));
				}
				return null;
			}, CustomWidgetsCacheTime);
		}

		/// <summary>
		/// Set custom widgets for the given area
		/// If `widgets` is null then remove the specification
		/// </summary>
		/// <param name="areaId">Area id</param>
		/// <param name="widgets">Custom widgets</param>
		public virtual void SetCustomWidgets(string areaId, IList<TemplateWidget> widgets) {
			// Remove cache
			CustomWidgetsCache.Remove(areaId);
			// Write to file
			var path = GetCustomWidgetsJsonPath(areaId);
			PathUtils.EnsureParentDirectory(path);
			if (widgets != null) {
				File.WriteAllText(path, JsonConvert.SerializeObject(widgets, Formatting.Indented));
			} else {
				File.Delete(path);
			}
		}

		/// <summary>
		/// Render widget
		/// Return reder result
		/// </summary>
		/// <param name="context">Template context</param>
		/// <param name="widget">Template widget</param>
		/// <returns></returns>
		public virtual string RenderWidget(Context context, TemplateWidget widget) {
			// Get from cache
			var info = GetWidgetInfo(widget.Path);
			IsolatedMemoryCache<string, string> renderCache = null;
			string key = null;
			string renderResult = null;
			if (info.CacheTime > 0) {
				renderCache = WidgetRenderCache.GetOrAdd(info.CacheBy ?? "",
					_ => new IsolatedMemoryCache<string, string>(info.GetCacheIsolationPolicyNames()));
				key = widget.GetCacheKey();
				renderResult = renderCache.GetOrDefault(key);
				if (renderResult != null) {
					return renderResult;
				}
			}
			// Render widget
			var writer = new StringWriter();
			writer.Write($"<div class='template_widget' data-widget='{key}'>");
			var scope = Hash.FromAnonymousObject(widget.Args);
			context.Stack(scope, () => {
				var includeTag = new Include();
				var htmlPath = widget.Path + TemplateWidgetInfo.HtmlExtension;
				includeTag.Initialize("include", htmlPath, null);
				includeTag.Render(context, writer);
			});
			writer.Write("</div>");
			renderResult = writer.ToString();
			// Store to cache
			if (info.CacheTime > 0) {
				renderCache.Put(key, renderResult, TimeSpan.FromSeconds(info.CacheTime));
			}
			return renderResult;
		}

		/// <summary>
		/// Clear cache
		/// </summary>
		public virtual void ClearCache() {
			WidgetInfoCache.Clear();
			CustomWidgetsCache.Clear();
			WidgetRenderCache.Clear();
		}
	}
}
