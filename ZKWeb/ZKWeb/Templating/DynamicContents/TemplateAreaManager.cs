using DotLiquid;
using DotLiquid.Tags;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ZKWeb.Cache;
using ZKWeb.Server;
using ZKWeb.Storage;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template area and widgets manager<br/>
	/// 模板区域和模块的管理器<br/>
	/// </summary>
	/// <seealso cref="TemplateArea"/>
	/// <seealso cref="TemplateWidget"/>
	/// <seealso cref="TemplateWidgetInfo"/>
	public class TemplateAreaManager : ICacheCleaner {
		/// <summary>
		/// Widget information cache time<br/>
		/// Default is 2s, able to override from website configuration<br/>
		/// 模块信息的缓存时间<br/>
		/// 默认是2秒, 可以根据网站配置覆盖<br/>
		/// </summary>
		public TimeSpan WidgetInfoCacheTime { get; set; }
		/// <summary>
		/// Custom widgets cache time<br/>
		/// Default is 2s, able to override from website configuration<br/>
		/// 自定义模块列表的缓存时间<br/>
		/// 默认是2秒, 可以根据网站配置覆盖<br/>
		/// </summary>
		public TimeSpan CustomWidgetsCacheTime { get; set; }
		/// <summary>
		/// Areas<br/>
		/// 区域列表<br/>
		/// { Area id: Area }<br/>
		/// </summary>
		protected Dictionary<string, TemplateArea> Areas { get; set; }
		/// <summary>
		/// Widget information cache<br/>
		/// Isolated by client device<br/>
		/// 模块信息的缓存<br/>
		/// 根据客户端设备类型隔离<br/>
		/// { Widget path: widget information }
		/// </summary>
		protected IKeyValueCache<string, TemplateWidgetInfo> WidgetInfoCache { get; set; }
		/// <summary>
		/// Custom widgets cache<br/>
		/// Isolated by client device<br/>
		/// 自定义模块列表的缓存<br/>
		/// 根据客户端设备类型隔离<br/>
		/// { Area id: custom widgets }
		/// </summary>
		protected IKeyValueCache<string, IList<TemplateWidget>> CustomWidgetsCache { get; set; }
		/// <summary>
		/// Widget render result cache<br/>
		/// 模块描画结果的缓存<br/>
		/// { Isolation policy: { Widget path and arguments: render result } }
		/// </summary>
		protected ConcurrentDictionary<string, IKeyValueCache<string, string>> WidgetRenderCache { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
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
		/// Get area by id<br/>
		/// If not found then create a new area with the given id<br/>
		/// 根据Id获取区域<br/>
		/// 如果找不到则创建一个新的区域, 设置id为指定id并返回<br/>
		/// </summary>
		/// <param name="areaId">Area id</param>
		/// <returns></returns>
		public virtual TemplateArea GetArea(string areaId) {
			return Areas.GetOrCreate(areaId, () => new TemplateArea(areaId));
		}

		/// <summary>
		/// Get widget information<br/>
		/// 获取模块信息<br/>
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
		/// Get json path that store custom widgets for the given area<br/>
		/// 获取保存指定区域的自定义模块列表的json文件<br/>
		/// Path: App_Data/areas/{areaId}.widgets<br/>
		/// </summary>
		/// <param name="areaId">Area id</param>
		/// <returns></returns>
		protected virtual IFileEntry GetCustomWidgetsFile(string areaId) {
			var fileStorage = Application.Ioc.Resolve<IFileStorage>();
			var path = fileStorage.GetStorageFile("areas", $"{areaId}.widgets");
			return path;
		}

		/// <summary>
		/// Get custom widgets for the given area<br/>
		/// Return null if custom widgets are unspecified<br/>
		/// 获取指定区域的自定义模块列表<br/>
		/// 如果未指定自定义模块列表则返回null<br/>
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
		/// Set custom widgets for the given area<br/>
		/// If parameter "widgets" is null then remove the old list<br/>
		/// 设置指定区域的自定义模块列表<br/>
		/// 如果参数"widgets"是null则删除旧的列表<br/>
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
		/// Render widget<br/>
		/// Return render result<br/>
		/// 描画模块<br/>
		/// 返回描画结果<br/>
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
			var renderer = Application.Ioc.Resolve<ITemplateWidgetRenderer>();
			renderResult = renderer.Render(context, widget);
			// Store to cache
			if (info.CacheTime > 0) {
				renderCache.Put(key, renderResult, TimeSpan.FromSeconds(info.CacheTime));
			}
			return renderResult;
		}

		/// <summary>
		/// Clear cache<br/>
		/// 清理缓存<br/>
		/// </summary>
		public virtual void ClearCache() {
			WidgetInfoCache.Clear();
			CustomWidgetsCache.Clear();
			WidgetRenderCache.Clear();
		}
	}
}
