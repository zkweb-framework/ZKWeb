using DotLiquid;
using DotLiquid.FileSystems;
using DryIoc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Model;
using ZKWeb.Utils.Collections;

namespace ZKWeb.Core {
	/// <summary>
	/// 模板系统使用的文件系统
	/// 使用PathManager.GetTemplateFullPath获取路径
	/// </summary>
	public class TemplateFileSystem : IFileSystem, ICacheCleaner {
		/// <summary>
		/// 模板缓存时间
		/// 这里的缓存时间是防止内存占用过多而设置的
		/// 无限期缓存也可以正常使用
		/// </summary>
		public TimeSpan TemplateCacheTime { get; set; } = TimeSpan.FromMinutes(15);
		/// <summary>
		/// 模板路径缓存时间
		/// 缓存用于减少硬盘查询次数，但时间不能超过1秒否则影响修改
		/// </summary>
		public TimeSpan TemplatePathCacheTime { get; set; } = TimeSpan.FromSeconds(1);
		/// <summary>
		/// 模板的缓存
		/// { 模板的绝对路径: (文件修改时间, 模板对象) }
		/// </summary>
		private MemoryCache<string, Tuple<DateTime, Template>> TemplateCache { get; set; } =
			new MemoryCache<string, Tuple<DateTime, Template>>();
		/// <summary>
		/// 模板路径的缓存
		/// { 模板名称: 模板的绝对路径 }
		/// </summary>
		private MemoryCache<string, string> TemplatePathCache { get; set; } =
			new MemoryCache<string, string>();

		/// <summary>
		/// 从模板路径读取模板
		/// </summary>
		/// <param name="context">上下文</param>
		/// <param name="templateName">模板路径</param>
		/// <returns></returns>
		public object ReadTemplateFile(Context context, string templateName) {
			// 从缓存获取模板的绝对路径
			// 没有时重新获取并设置到缓存
			var fullPath = TemplatePathCache.GetOrDefault(templateName);
			if (fullPath == null) {
				var pathManager = Application.Ioc.Resolve<PathManager>();
				fullPath = pathManager.GetTemplateFullPath(templateName);
				if (fullPath == null) {
					return null;
				}
				TemplatePathCache.Put(templateName, fullPath, TemplatePathCacheTime);
			}
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
		public void ClearCache() {
			TemplateCache.Clear();
			TemplatePathCache.Clear();
		}
	}
}
