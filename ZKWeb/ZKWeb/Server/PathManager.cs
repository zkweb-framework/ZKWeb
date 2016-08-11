using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZKWeb.Cache;
using ZKWeb.Plugin;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Web;

namespace ZKWeb.Server {
	/// <summary>
	/// Path manager
	/// </summary>
	public class PathManager : ICacheCleaner {
		/// <summary>
		/// Template path cache time
		/// Default is 2s, able to override from website configuration
		/// </summary>
		public TimeSpan TemplatePathCacheTime { get; set; }
		/// <summary>
		/// Resource path cache time
		/// Default is 2s, able to override from website configuration
		/// </summary>
		public TimeSpan ResourcePathCacheTime { get; set; }
		/// <summary>
		/// Template path cache
		/// Isolated by client device
		/// { path: absolute path }
		/// </summary>
		protected IsolatedMemoryCache<string, string> TemplatePathCache { get; set; }
		/// <summary>
		/// Resource path cache
		/// Isolated by client device
		/// { path: absolute path }
		/// </summary>
		protected IsolatedMemoryCache<string, string> ResourcePathCache { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public PathManager() {
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			TemplatePathCacheTime = TimeSpan.FromSeconds(
				configManager.WebsiteConfig.Extra.GetOrDefault(ExtraConfigKeys.TemplatePathCacheTime, 2));
			ResourcePathCacheTime = TimeSpan.FromSeconds(
				configManager.WebsiteConfig.Extra.GetOrDefault(ExtraConfigKeys.ResourcePathCacheTime, 2));
			TemplatePathCache = new IsolatedMemoryCache<string, string>("Device");
			ResourcePathCache = new IsolatedMemoryCache<string, string>("Device");
		}

		/// <summary>
		/// Get plugin root directories in absolute path
		/// These directories are use to find plugins
		/// </summary>
		/// <returns></returns>
		public virtual IList<string> GetPluginDirectories() {
			var pathConfig = Application.Ioc.Resolve<PathConfig>();
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			return configManager.WebsiteConfig.PluginDirectories.Select(p =>
				Path.GetFullPath(Path.Combine(pathConfig.WebsiteRootDirectory, p))).ToList();
		}

		/// <summary>
		/// Get template full path candidates
		/// Rules
		/// Explicit plugin name
		/// - Format "PluginName:TemplatePath"
		/// - Example: "Common.Base:include/header.html"
		/// - Path candidates
		///   - {PluginDirectory}\templates.{device}\{TemplatePath}
		///   - {PluginDirectory}\templates\{TemplatePath}
		/// No Explicit plugin name
		/// - Example "include/header.html"
		/// - Path candidates
		///   - App_Data\templates.{device}\{TemplatePath}
		///   - Iterate plugins in reverse order
		///     - {PluginDirectory}\templates.{device}\{TemplatePath}
		///   - App_Data\templates\{TemplatePath}
		///   - Iterate plugins in reverse order
		///     - {PluginDirectory}\templates\{TemplatePath}
		/// </summary>
		/// <param name="path">Template path</param>
		/// <returns></returns>
		public virtual IEnumerable<string> GetTemplateFullPathCandidates(string path) {
			// Get client device and specialized template directory name
			var pathConfig = Application.Ioc.Resolve<PathConfig>();
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
		/// Get template full path
		/// Return null if template file not found
		/// It will cache result within a short time
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
		/// Get resource full path candidates
		/// Path candidates
		/// - App_Data\{ResourcePath}
		/// - Iterate plugins in reverse order
		///   - {PluginDirectory}\{ResourcePath}
		/// </summary>
		/// <param name="pathParts">Resource path</param>
		/// <returns></returns>
		public virtual IEnumerable<string> GetResourceFullPathCandidates(params string[] pathParts) {
			// Load from App_Data first
			var path = PathUtils.SecureCombine(pathParts);
			var pathConfig = Application.Ioc.Resolve<PathConfig>();
			yield return PathUtils.SecureCombine(pathConfig.AppDataDirectory, path);
			// Then load from plugin directories
			var pluginManager = Application.Ioc.Resolve<PluginManager>();
			foreach (var plugin in pluginManager.Plugins.Reverse<PluginInfo>()) {
				yield return PathUtils.SecureCombine(plugin.Directory, path);
			}
		}

		/// <summary>
		/// Get resource full
		/// Return null if resource file not found
		/// It will cache result within a short time
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
		/// Get storage file full path
		/// Return App_Data\{StoragePath} either file exist or not
		/// </summary>
		/// <param name="pathParts">Storage path</param>
		/// <returns></returns>
		public virtual string GetStorageFullPath(params string[] pathParts) {
			var path = PathUtils.SecureCombine(pathParts);
			var pathConfig = Application.Ioc.Resolve<PathConfig>();
			var fullPath = PathUtils.SecureCombine(pathConfig.AppDataDirectory, path);
			return fullPath;
		}

		/// <summary>
		/// Clear cache
		/// </summary>
		public virtual void ClearCache() {
			TemplatePathCache.Clear();
			ResourcePathCache.Clear();
		}
	}
}
