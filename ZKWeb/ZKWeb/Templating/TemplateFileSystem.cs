using DotLiquid;
using DotLiquid.FileSystems;
using System;
using System.IO;
using ZKWeb.Cache;
using ZKWeb.Server;
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
		protected MemoryCache<string, Pair<Template, DateTime>> TemplateCache { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public TemplateFileSystem() {
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			TemplateCacheTime = TimeSpan.FromSeconds(
				configManager.WebsiteConfig.Extra.GetOrDefault(ExtraConfigKeys.TemplateCacheTime, 180));
			TemplateCache = new MemoryCache<string, Pair<Template, DateTime>>();
		}

		/// <summary>
		/// Read template object from path
		/// </summary>
		/// <param name="context">Template context</param>
		/// <param name="templateName">Template path</param>
		/// <returns></returns>
		public virtual object ReadTemplateFile(Context context, string templateName) {
			// Get template full path
			var pathManager = Application.Ioc.Resolve<PathManager>();
			var fullPath = pathManager.GetTemplateFullPath(templateName);
			if (fullPath == null) {
				throw new FileNotFoundException(
					string.Format("template {0} not found", templateName));
			}
			// Get parsed object from cache
			var lastWriteTime = File.GetLastWriteTimeUtc(fullPath);
			var cache = TemplateCache.GetOrDefault(fullPath);
			if (cache.First != null && cache.Second == lastWriteTime) {
				return cache.First;
			}
			// Parse template and store to cache
			var sources = File.ReadAllText(fullPath);
			var template = Template.Parse(sources);
			cache = Pair.Create(template, lastWriteTime);
			TemplateCache.Put(fullPath, cache, TemplateCacheTime);
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
