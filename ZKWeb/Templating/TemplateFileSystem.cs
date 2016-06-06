using DotLiquid;
using DotLiquid.FileSystems;
using DryIoc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Cache.Interfaces;
using ZKWeb.Server;
using ZKWeb.Utils.Collections;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Templating {
	/// <summary>
	/// 模板系统使用的文件系统
	/// 使用PathManager.GetTemplateFullPath获取模板路径
	/// </summary>
	public class TemplateFileSystem : IFileSystem, ICacheCleaner {
		/// <summary>
		/// 模板缓存时间
		/// 默认是180秒，可通过网站配置指定
		/// </summary>
		public TimeSpan TemplateCacheTime { get; set; }
		/// <summary>
		/// 模板的缓存
		/// { 模板的绝对路径: (文件修改时间, 模板对象) }
		/// </summary>
		protected MemoryCache<string, Tuple<DateTime, Template>> TemplateCache { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		public TemplateFileSystem() {
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			TemplateCacheTime = TimeSpan.FromSeconds(
				configManager.WebsiteConfig.Extra.GetOrDefault(ExtraConfigKeys.TemplateCacheTime, 180));
			TemplateCache = new MemoryCache<string, Tuple<DateTime, Template>>();
		}

		/// <summary>
		/// 从模板路径读取模板
		/// </summary>
		/// <param name="context">上下文</param>
		/// <param name="templateName">模板路径</param>
		/// <returns></returns>
		public virtual object ReadTemplateFile(Context context, string templateName) {
			// 获取模板的绝对路径
			var pathManager = Application.Ioc.Resolve<PathManager>();
			var fullPath = pathManager.GetTemplateFullPath(templateName);
			// 从缓存获取模板
			var lastWriteTime = File.GetLastWriteTimeUtc(fullPath);
			var cache = TemplateCache.GetOrDefault(fullPath);
			if (cache != null && cache.Item1 == lastWriteTime) {
				return cache.Item2;
			}
			// 解析模板并保存到缓存
			var sources = File.ReadAllText(fullPath);
			var template = Template.Parse(sources);
			TemplateCache.Put(fullPath, Tuple.Create(lastWriteTime, template), TemplateCacheTime);
			return template;
		}

		/// <summary>
		/// 清理缓存
		/// </summary>
		public virtual void ClearCache() {
			TemplateCache.Clear();
		}
	}
}
