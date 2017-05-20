using DotLiquid;
using DotLiquid.FileSystems;
using System;
using System.IO;
using ZKWeb.Cache;
using ZKWeb.Server;
using ZKWeb.Storage;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;

namespace ZKWeb.Templating {
	/// <summary>
	/// Dotliquid template file system<br/>
	/// Dotliquid使用的模板文件系统<br/>
	/// </summary>
	/// <seealso cref="TemplateManager"/>
	public class TemplateFileSystem : IFileSystem, ICacheCleaner {
		/// <summary>
		/// Parsed template cache time<br/>
		/// Default is 180s, able to override from website configuration<br/>
		/// 已解析模板的缓存时间<br/>
		/// 默认是180秒, 可以根据网站配置覆盖<br/>
		/// </summary>
		public TimeSpan TemplateCacheTime { get; set; }
		/// <summary>
		/// Parsed template cache<br/>
		/// 已解析模板的缓存<br/>
		/// { Full path: (Template object, Modify time) }<br/>
		/// </summary>
		protected IKeyValueCache<string, Pair<Template, DateTime>> TemplateCache { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public TemplateFileSystem() {
			var configManager = Application.Ioc.Resolve<WebsiteConfigManager>();
			var cacheFactory = Application.Ioc.Resolve<ICacheFactory>();
			TemplateCacheTime = TimeSpan.FromSeconds(
				configManager.WebsiteConfig.Extra.GetOrDefault(ExtraConfigKeys.TemplateCacheTime, 180));
			TemplateCache = cacheFactory.CreateCache<string, Pair<Template, DateTime>>();
		}

		/// <summary>
		/// Read template object from path<br/>
		/// 从路径读取模板对象<br/>
		/// </summary>
		/// <param name="context">Template context</param>
		/// <param name="templateName">Template path</param>
		/// <returns></returns>
		public virtual object ReadTemplateFile(Context context, string templateName) {
			// Get template full path
			var fileStorage = Application.Ioc.Resolve<IFileStorage>();
			var templateFile = fileStorage.GetTemplateFile(templateName);
			if (!templateFile.Exists) {
				throw new FileNotFoundException(
					string.Format("template {0} not found", templateName));
			}
			// Get parsed object from cache
			var lastWriteTime = templateFile.LastWriteTimeUtc;
			var cache = TemplateCache.GetOrDefault(templateFile.UniqueIdentifier);
			if (cache.First != null && cache.Second == lastWriteTime) {
				return cache.First;
			}
			// Parse template and store to cache
			var sources = templateFile.ReadAllText();
			var template = Template.Parse(sources);
			cache = Pair.Create(template, lastWriteTime);
			TemplateCache.Put(templateFile.UniqueIdentifier, cache, TemplateCacheTime);
			return template;
		}

		/// <summary>
		/// Clear cache<br/>
		/// 清理缓存<br/>
		/// </summary>
		public virtual void ClearCache() {
			TemplateCache.Clear();
		}
	}
}
