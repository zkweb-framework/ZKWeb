using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Plugin.Interfaces;
using ZKWeb.Utils.Collections;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Templating.Diy {
	/// <summary>
	/// 主要提供区域(Area)和模块(Widget)和可视化编辑的管理
	/// 这个管理器和TemplateManager不同的是，
	/// TemplateManager管理的是模板文件本身而这个类针对Area和Widget标签管理
	/// </summary>
	public class DiyManager : ICacheCleaner {
		/// <summary>
		/// 模块信息的缓存时间
		/// 缓存用于减少硬盘查询次数，但时间不能超过1秒否则影响修改
		/// </summary>
		public TimeSpan WidgetInfoCacheTime { get; set; } = TimeSpan.FromSeconds(1);
		/// <summary>
		/// 已知的区域列表
		/// </summary>
		private Dictionary<string, DiyArea> Areas { get; } =
			new Dictionary<string, DiyArea>();
		/// <summary>
		/// Diy模块描画结果的缓存
		/// { 模块名称和参数: 描画结果, ... }
		/// </summary>
		private MemoryCache<string, string> WidgetRenderCache { get; } =
			new MemoryCache<string, string>();
		/// <summary>
		/// Diy模块信息的缓存
		/// { 模块名称: 模块信息, ... }
		/// </summary>
		private MemoryCache<string, DiyWidgetInfo> WidgetInfoCache { get; } =
			new MemoryCache<string, DiyWidgetInfo>();

		/// <summary>
		/// 获取区域
		/// 这个函数不会返回null
		/// </summary>
		/// <param name="id">区域Id</param>
		/// <returns></returns>
		public virtual DiyArea GetArea(string id) {
			return Areas.GetOrCreate(id, () => new DiyArea(id));
		}

		/// <summary>
		/// 获取Diy模块描画结果的缓存
		/// </summary>
		/// <param name="widgetPath">模块名称</param>
		/// <param name="widgetArgs">模块参数</param>
		/// <returns></returns>
		public virtual string GetWidgetRenderCache(string widgetPath, string widgetArgs) {
			var key = widgetPath + " " + widgetArgs;
			return WidgetRenderCache.GetOrDefault(key);
		}

		/// <summary>
		/// 设置Diy模块描画结果的缓存
		/// 缓存时间会自动从模块信息中获取
		/// </summary>
		/// <param name="widgetPath">模块名称</param>
		/// <param name="widgetArgs">模块参数</param>
		/// <param name="renderResult">描画结果</param>
		public virtual void PutWidgetRenderCache(string widgetPath, string widgetArgs, string renderResult) {
			var key = widgetPath + " " + widgetArgs;
			var info = WidgetInfoCache.GetOrDefault(widgetPath);
			if (info == null) {
				info = DiyWidgetInfo.FromPath(widgetPath);
				WidgetInfoCache.Put(widgetPath, info, WidgetInfoCacheTime);
			}
			if (info.CacheTime > 0) {
				WidgetRenderCache.Put(key, renderResult, TimeSpan.FromSeconds(info.CacheTime));
			}
		}

		/// <summary>
		/// 清理缓存
		/// </summary>
		public void ClearCache() {
			WidgetInfoCache.Clear();
			WidgetRenderCache.Clear();
		}
	}
}
