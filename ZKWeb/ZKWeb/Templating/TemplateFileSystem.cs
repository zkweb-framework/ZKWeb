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
	/// Dotliquid template file system
	/// </summary>
	public class TemplateFileSystem : IFileSystem, ICacheCleaner {
		/// <summary>
		/// Parsed template cache time
		/// Default is 180s, able to override from website configuration
		/// </summary>
		public TimeSpan TemplateCacheTime { get; set; }
		/// <summary>
		/// Parsed template cache
		/// { Full path: (Template object, Modify time) }
		/// </summary>
		protected IKeyValueCache<string, Pair<Template, DateTime>> TemplateCache { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public TemplateFileSystem() {
			var configManager = Application.Ioc.Resolve<WebsiteConfigManager>();
			var cacheFactory = Application.Ioc.Resolve<ICacheFactory>();
			TemplateCacheTime = TimeSpan.FromSeconds(
				configManager.WebsiteConfig.Extra.GetOrDefault(ExtraConfigKeys.TemplateCacheTime, 180));
			TemplateCache = cacheFactory.CreateCache<string, Pair<Template, DateTime>>();
		}

		/// <summary>
		/// Read template object from path
		/// </summary>
		/// <param name="context">Template context</param>
		/// <param name="templateName">Template path</param>
		/// <returns></returns>
		public virtual object ReadTemplateFile(Context context, string templateName) {
			// Get template full path
			var fileStorage = Application.Ioc.Resolve<IFileStorage>();
			var templateFile = fileStorage.GetTemplateFile(templateName);
			if (!templateFile.Exist) {
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
		/// Clear cache
		/// </summary>
		public virtual void ClearCache() {
			TemplateCache.Clear();
		}
	}
}
