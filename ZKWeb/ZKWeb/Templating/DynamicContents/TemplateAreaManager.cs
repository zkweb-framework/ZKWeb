using DotLiquid;
using DotLiquid.Tags;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using ZKWeb.Cache;
using ZKWeb.Server;
using ZKWeb.Storage;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;

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
		protected IKeyValueCache<string, TemplateWidgetInfo> WidgetInfoCache { get; set; }
		/// <summary>
		/// Custom widgets cache
		/// Isolated by client device
		/// { Area id: custom widgets }
		/// </summary>
		protected IKeyValueCache<string, IList<TemplateWidget>> CustomWidgetsCache { get; set; }
		/// <summary>
		/// Widget render result cache
		/// { Isolation policy: { Width path and arguments: render result } }
		/// </summary>
		protected ConcurrentDictionary<string, IKeyValueCache<string, string>> WidgetRenderCache { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public TemplateAreaManager() {
			var configManager = Application.Ioc.Resolve<WebsiteConfigManager>();
			var cacheFactory = Application.Ioc.Resolve<ICacheFactory>();
			WidgetInfoCacheTime = TimeSpan.FromSeconds(
				configManager.WebsiteConfig.Extra.GetOrDefault(ExtraConfigKeys.WidgetInfoCacheTime, 2));
			CustomWidgetsCacheTime = TimeSpan.FromSeconds(
				configManager.WebsiteConfig.Extra.GetOrDefault(ExtraConfigKeys.CustomWidgetsCacheTime, 2));
			Areas = new Dictionary<string, TemplateArea>();
			WidgetInfoCache = cacheFactory.CreateCache<string, TemplateWidgetInfo>(
				CacheFactoryOptions.Default.WithIsolationPolicies("Device"));
			CustomWidgetsCache = cacheFactory.CreateCache<string, IList<TemplateWidget>>(
				CacheFactoryOptions.Default.WithIsolationPolicies("Device"));
			WidgetRenderCache = new ConcurrentDictionary<string, IKeyValueCache<string, string>>();
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
		protected virtual IFileEntry GetCustomWidgetsFile(string areaId) {
			var fileStorage = Application.Ioc.Resolve<IFileStorage>();
			var path = fileStorage.GetStorageFile("areas", $"{areaId}.widgets");
			return path;
		}

		/// <summary>
		/// Get custom widgets for the given area
		/// Return null if custom widgets are unspecified
		/// </summary>
		/// <param name="areaId">Area id</param>
		/// <returns></returns>
		public virtual IList<TemplateWidget> GetCustomWidgets(string areaId) {
			return CustomWidgetsCache.GetOrCreate(areaId, () => {
				var file = GetCustomWidgetsFile(areaId);
				if (file.Exists) {
					return JsonConvert.DeserializeObject<List<TemplateWidget>>(file.ReadAllText());
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
			var file = GetCustomWidgetsFile(areaId);
			if (widgets != null) {
				file.WriteAllText(JsonConvert.SerializeObject(widgets, Formatting.Indented));
			} else {
				file.Delete();
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
			IKeyValueCache<string, string> renderCache = null;
			string key = widget.GetCacheKey();
			string renderResult = null;
			if (info.CacheTime > 0) {
				renderCache = WidgetRenderCache.GetOrAdd(info.CacheBy ?? "", _ => {
					var cacheFactory = Application.Ioc.Resolve<ICacheFactory>();
					var policyNames = info.GetCacheIsolationPolicyNames();
					return cacheFactory.CreateCache<string, string>(
						CacheFactoryOptions.Default.WithIsolationPolicies(policyNames));
				});
				renderResult = renderCache.GetOrDefault(key);
				if (renderResult != null) {
					return renderResult;
				}
			}
			// Render widget
			var templateManager = Application.Ioc.Resolve<TemplateManager>();
			var writer = new StringWriter();
			writer.Write($"<div class='template_widget' data-widget='{key}'>");
			var scope = templateManager.CreateHash(widget.Args);
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
