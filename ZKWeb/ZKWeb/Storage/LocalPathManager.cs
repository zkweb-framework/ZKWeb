using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZKWeb.Cache;
using ZKWeb.Plugin;
using ZKWeb.Server;
using ZKWebStandard.Collections;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWeb.Storage {
	/// <summary>
	/// Local path manager<br/>
	/// It's better to use IFileStorage<br/>
	/// Unless you really want to access local file system<br/>
	/// 本地路径管理器<br/>
	/// 一般情况最好使用IFileStorage, 除非你确实想访问本地文件系统<br/>
	/// </summary>
	public class LocalPathManager : ICacheCleaner {
		/// <summary>
		/// Template path cache time<br/>
		/// Default is 2s, able to override from website configuration<br/>
		/// 模板路径的缓存时间<br/>
		/// 默认是2秒, 可以根据网站配置覆盖<br/>
		/// </summary>
		public TimeSpan TemplatePathCacheTime { get; set; }
		/// <summary>
		/// Resource path cache time<br/>
		/// Default is 2s, able to override from website configuration<br/>
		/// 资源路径的缓存时间<br/>
		/// 默认是2秒, 可以根据网站配置覆盖<br/>
		/// </summary>
		public TimeSpan ResourcePathCacheTime { get; set; }
		/// <summary>
		/// Template path cache<br/>
		/// Isolated by client device<br/>
		/// 模板路径的缓存<br/>
		/// 按客户端设备类型隔离<br/>
		/// { path: absolute path }
		/// </summary>
		protected IKeyValueCache<string, string> TemplatePathCache { get; set; }
		/// <summary>
		/// Resource path cache<br/>
		/// Isolated by client device<br/>
		/// 资源路径的缓存<br/>
		/// 按客户端设备类型隔离<br/>
		/// { path: absolute path }
		/// </summary>
		protected IKeyValueCache<string, string> ResourcePathCache { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public LocalPathManager() {
			var configManager = Application.Ioc.Resolve<WebsiteConfigManager>();
			TemplatePathCacheTime = TimeSpan.FromSeconds(
				configManager.WebsiteConfig.Extra.GetOrDefault(ExtraConfigKeys.TemplatePathCacheTime, 2));
			ResourcePathCacheTime = TimeSpan.FromSeconds(
				configManager.WebsiteConfig.Extra.GetOrDefault(ExtraConfigKeys.ResourcePathCacheTime, 2));
			// Path may different between servers, shouldn't use ICacheFactory here
			TemplatePathCache = new IsolatedKeyValueCache<string, string>(
				new[] { Application.Ioc.Resolve<ICacheIsolationPolicy>(serviceKey: "Device") },
				new MemoryCache<IsolatedCacheKey<string>, string>());
			ResourcePathCache = new IsolatedKeyValueCache<string, string>(
				new[] { Application.Ioc.Resolve<ICacheIsolationPolicy>(serviceKey: "Device") },
				new MemoryCache<IsolatedCacheKey<string>, string>());
		}

		/// <summary>
		/// Get plugin root directories in absolute path<br/>
		/// These directories are use to find plugins<br/>
		/// 获取保存插件的目录的绝对路径列表<br/>
		/// 这些路径会用于查找插件<br/>
		/// </summary>
		/// <returns></returns>
		public virtual IList<string> GetPluginDirectories() {
			var pathConfig = Application.Ioc.Resolve<LocalPathConfig>();
			var configManager = Application.Ioc.Resolve<WebsiteConfigManager>();
			return configManager.WebsiteConfig.PluginDirectories.Select(p =>
				Path.GetFullPath(Path.Combine(pathConfig.WebsiteRootDirectory, p))).ToList();
		}

		/// <summary>
		/// Get template full path candidates<br/>
		/// 获取模板文件的完整路径候选列表<br/>
		/// Rules<br/>
		/// 规则<br/>
		/// Explicit plugin name<br/>
		/// 显式指定插件名称<br/>
		/// - Format "PluginName:TemplatePath"<br/>
		/// - Example: "Common.Base:include/header.html"<br/>
		/// - Path candidates<br/>
		///   - {PluginDirectory}\templates.{device}\{TemplatePath}<br/>
		///   - {PluginDirectory}\templates\{TemplatePath}<br/>
		/// No Explicit plugin name<br/>
		/// 不指定插件名称<br/>
		/// - Example "include/header.html"<br/>
		/// - Path candidates<br/>
		///   - App_Data\templates.{device}\{TemplatePath}<br/>
		///   - Iterate plugins in reverse order<br/>
		///     - {PluginDirectory}\templates.{device}\{TemplatePath}<br/>
		///   - App_Data\templates\{TemplatePath}<br/>
		///   - Iterate plugins in reverse order<br/>
		///     - {PluginDirectory}\templates\{TemplatePath}<br/>
		/// </summary>
		/// <param name="path">Template path</param>
		/// <returns></returns>
		public virtual IEnumerable<string> GetTemplateFullPathCandidates(string path) {
			// Get client device and specialized template directory name
			var pathConfig = Application.Ioc.Resolve<LocalPathConfig>();
			var device = HttpManager.CurrentContext.GetClientDevice();
			var deviceSpecializedTemplateDirectoryName = string.Format(
				pathConfig.DeviceSpecializedTemplateDirectoryNameFormat, device.ToString().ToLower());
			// Get plugin name if explicitly specified
			var index = path.IndexOf(':');
			string explicitPlugin = null;
			if (index >= 0) {
				explicitPlugin = path.Substring(0, index);
				path = path.Substring(index + 1); // 这里可以等于字符串长度
			}
			// Get full path
			if (explicitPlugin != null) {
				// If plugin name explicitly specified
				foreach (var pluginDirectory in GetPluginDirectories()) {
					yield return PathUtils.SecureCombine(
						pluginDirectory, explicitPlugin, deviceSpecializedTemplateDirectoryName, path);
					yield return PathUtils.SecureCombine(
						pluginDirectory, explicitPlugin, pathConfig.TemplateDirectoryName, path);
				}
			} else {
				// If plugin name not specified
				// Load from device specialized template directories first
				yield return PathUtils.SecureCombine(
					pathConfig.AppDataDirectory, deviceSpecializedTemplateDirectoryName, path);
				var pluginManager = Application.Ioc.Resolve<PluginManager>();
				foreach (var plugin in pluginManager.Plugins.Reverse<PluginInfo>()) {
					yield return PathUtils.SecureCombine(
						plugin.Directory, deviceSpecializedTemplateDirectoryName, path);
				}
				// Then load from default template directories
				yield return PathUtils.SecureCombine(
					pathConfig.AppDataDirectory, pathConfig.TemplateDirectoryName, path);
				foreach (var plugin in pluginManager.Plugins.Reverse<PluginInfo>()) {
					yield return PathUtils.SecureCombine(
						plugin.Directory, pathConfig.TemplateDirectoryName, path);
				}
			}
		}

		/// <summary>
		/// Get template full path<br/>
		/// Return null if template file not found<br/>
		/// Result will cache for a short time<br/>
		/// 获取模板文件的完整路径<br/>
		/// 如果找不到则返回null<br/>
		/// 结果会在短暂的时间内缓存<br/>
		/// </summary>
		/// <param name="path">Template path</param>
		/// <returns></returns>
		public virtual string GetTemplateFullPath(string path) {
			var fullPath = TemplatePathCache.GetOrDefault(path);
			if (fullPath == null) {
				fullPath = GetTemplateFullPathCandidates(path).FirstOrDefault(p => File.Exists(p));
				if (fullPath != null) {
					TemplatePathCache.Put(path, fullPath, TemplatePathCacheTime);
				}
			}
			return fullPath;
		}

		/// <summary>
		/// Get resource full path candidates<br/>
		/// 获取资源文件的完整路径的候选列表<br/>
		/// Path candidates<br/>
		/// - App_Data\{ResourcePath}<br/>
		/// - Iterate plugins in reverse order<br/>
		///   - {PluginDirectory}\{ResourcePath}<br/>
		/// </summary>
		/// <param name="pathParts">Resource path</param>
		/// <returns></returns>
		public virtual IEnumerable<string> GetResourceFullPathCandidates(params string[] pathParts) {
			// Load from App_Data first
			var path = PathUtils.SecureCombine(pathParts);
			var pathConfig = Application.Ioc.Resolve<LocalPathConfig>();
			yield return PathUtils.SecureCombine(pathConfig.AppDataDirectory, path);
			// Then load from plugin directories
			var pluginManager = Application.Ioc.Resolve<PluginManager>();
			foreach (var plugin in pluginManager.Plugins.Reverse<PluginInfo>()) {
				yield return PathUtils.SecureCombine(plugin.Directory, path);
			}
		}

		/// <summary>
		/// Get resource full path<br/>
		/// Return null if resource file not found<br/>
		/// Result will cache for a short time<br/>
		/// 获取资源文件的完整路径<br/>
		/// 如果找不到则返回null<br/>
		/// 结果会在短暂的时间内缓存<br/>
		/// </summary>
		/// <param name="pathParts">Resource path</param>
		/// <returns></returns>
		public virtual string GetResourceFullPath(params string[] pathParts) {
			var key = string.Join("/", pathParts);
			var fullPath = ResourcePathCache.GetOrDefault(key);
			if (fullPath == null) {
				fullPath = GetResourceFullPathCandidates(pathParts).FirstOrDefault(p => File.Exists(p));
				if (fullPath != null) {
					ResourcePathCache.Put(key, fullPath, TemplatePathCacheTime);
				}
			}
			return fullPath;
		}

		/// <summary>
		/// Get storage file or directory full path<br/>
		/// Return App_Data\{StoragePath} either exist or not exist<br/>
		/// 获取储存文件或文件夹的完整路径<br/>
		/// 返回App_Data\{储存路径}, 无论是否存在<br/>
		/// </summary>
		/// <param name="pathParts">Storage path</param>
		/// <returns></returns>
		public virtual string GetStorageFullPath(params string[] pathParts) {
			var path = PathUtils.SecureCombine(pathParts);
			var pathConfig = Application.Ioc.Resolve<LocalPathConfig>();
			var fullPath = PathUtils.SecureCombine(pathConfig.AppDataDirectory, path);
			return fullPath;
		}

		/// <summary>
		/// Clear cache<br/>
		/// 清理缓存<br/>
		/// </summary>
		public virtual void ClearCache() {
			TemplatePathCache.Clear();
			ResourcePathCache.Clear();
		}
	}
}
