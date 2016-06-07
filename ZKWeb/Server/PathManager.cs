using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Cache;
using ZKWeb.Cache.Interfaces;
using ZKWeb.Plugin;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Server {
	/// <summary>
	/// 路径管理器
	/// </summary>
	public class PathManager : ICacheCleaner {
		/// <summary>
		/// 模板路径缓存时间
		/// 默认是2秒，可通过网站配置指定
		/// </summary>
		public TimeSpan TemplatePathCacheTime { get; set; }
		/// <summary>
		/// 资源路径的缓存时间
		/// 默认是2秒，可通过网站配置指定
		/// </summary>
		public TimeSpan ResourcePathCacheTime { get; set; }
		/// <summary>
		/// 模板路径的缓存
		/// 按当前设备隔离
		/// { 模板名称: 模板的绝对路径 }
		/// </summary>
		protected IsolatedMemoryCache<string, string> TemplatePathCache { get; set; }
		/// <summary>
		/// 资源路径的缓存
		/// 按当前设备隔离
		/// { 资源路径: 资源的绝对路径 }
		/// </summary>
		protected IsolatedMemoryCache<string, string> ResourcePathCache { get; set; }

		/// <summary>
		/// 初始化
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
		/// 获取插件根目录的绝对路径
		/// 网站目录 + 网站配置中定义的插件根目录的相对路径
		/// </summary>
		/// <returns></returns>
		public virtual List<string> GetPluginDirectories() {
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			return configManager.WebsiteConfig.PluginDirectories.Select(p =>
				Path.GetFullPath(Path.Combine(PathUtils.WebRoot.Value, p))).ToList();
		}

		/// <summary>
		/// 获取模板的完整路径的候选列表
		/// 规则
		///	显式指定插件，这时不允许从其他插件或App_Data重载模板
		///	- "所在插件:模板路径"
		/// - 例 "Common.Base:include/header.html"
		/// - 模板路径
		///   - 插件目录\templates.{device}\模板路径
		///   - 插件目录\templates\模板路径
		///   - 显式指定插件通常用于模板的继承
		/// 不指定插件，允许其他插件或App_Data重载模板
		/// - "模板路径"
		/// - 例 "include/header.html"
		/// - 查找模板路径的顺序
		///   - App_Data\templates.{device}\模板路径
		///   - App_Data\templates\模板路径
		///   - 按载入顺序反向枚举插件
		///     - 插件目录\templates.{device}\模板路径
		///     - 插件目录\templates\模板路径
		/// 这个函数会按以上规则逐条返回路径
		/// </summary>
		/// <param name="path">模板路径</param>
		/// <returns></returns>
		public virtual IEnumerable<string> GetTemplateFullPathCandidates(string path) {
			// 获取当前设备和设备专用的模板文件夹的名称
			var device = HttpDeviceUtils.GetClientDevice();
			var deviceSpecializedTemplateDirectoryName = string.Format(
				PathConfig.DeviceSpecializedTemplateDirectoryNameFormat, device.ToString().ToLower());
			// 获取显式指定的插件，没有时explictPlugin会等于null
			var index = path.IndexOf(':');
			string explictPlugin = null;
			if (index >= 0) {
				explictPlugin = path.Substring(0, index);
				path = path.Substring(index + 1); // 这里可以等于字符串长度
			}
			// 获取完整路径
			if (explictPlugin != null) {
				// 显式指定插件时
				foreach (var pluginDirectory in GetPluginDirectories()) {
					yield return PathUtils.SecureCombine(
						pluginDirectory, explictPlugin, deviceSpecializedTemplateDirectoryName, path);
					yield return PathUtils.SecureCombine(
						pluginDirectory, explictPlugin, PathConfig.TemplateDirectoryName, path);
				}
			} else {
				// 不指定插件时，先从App_Data获取
				yield return PathUtils.SecureCombine(
					PathConfig.AppDataDirectory, deviceSpecializedTemplateDirectoryName, path);
				yield return PathUtils.SecureCombine(
					PathConfig.AppDataDirectory, PathConfig.TemplateDirectoryName, path);
				// 从各个插件目录获取，按载入顺序反向枚举
				var pluginManager = Application.Ioc.Resolve<PluginManager>();
				foreach (var plugin in pluginManager.Plugins.Reverse<PluginInfo>()) {
					yield return PathUtils.SecureCombine(
						plugin.Directory, deviceSpecializedTemplateDirectoryName, path);
					yield return PathUtils.SecureCombine(
						plugin.Directory, PathConfig.TemplateDirectoryName, path);
				}
			}
		}

		/// <summary>
		/// 获取模板的完整路径
		/// 没有找到存在的模板文件路径时返回null
		/// 这个函数使用了缓存，详细说明请见GetTemplateFullPathCandidates
		/// </summary>
		/// <param name="path">模板路径</param>
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
		/// 获取资源文件的完整路径的候选列表
		///	查找路径的顺序
		/// - App_Data\文件路径
		/// - 按载入顺序反向枚举插件
		///   - 插件目录\文件路径
		/// 这个函数会按以上规则逐条返回路径
		/// </summary>
		/// <param name="pathParts">路径</param>
		/// <returns></returns>
		public virtual IEnumerable<string> GetResourceFullPathCandidates(params string[] pathParts) {
			// 先从App_Data获取
			var path = PathUtils.SecureCombine(pathParts);
			yield return PathUtils.SecureCombine(PathConfig.AppDataDirectory, path);
			// 从各个插件目录获取，按载入顺序反向枚举
			var pluginManager = Application.Ioc.Resolve<PluginManager>();
			foreach (var plugin in pluginManager.Plugins.Reverse<PluginInfo>()) {
				yield return PathUtils.SecureCombine(plugin.Directory, path);
			}
		}

		/// <summary>
		/// 获取资源文件的完整路径
		/// 没有找到存在的资源文件路径时返回null
		/// 这个函数使用了缓存，详细说明请见GetResourceFullPathCandidates
		/// </summary>
		/// <param name="pathParts">路径</param>
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
		/// 获取储存文件的完整路径
		/// 无论文件是否存在，返回App_Data\文件路径
		/// </summary>
		/// <param name="pathParts">路径</param>
		/// <returns></returns>
		public virtual string GetStorageFullPath(params string[] pathParts) {
			var path = PathUtils.SecureCombine(pathParts);
			var fullPath = PathUtils.SecureCombine(PathConfig.AppDataDirectory, path);
			return fullPath;
		}

		/// <summary>
		/// 清理缓存
		/// </summary>
		public virtual void ClearCache() {
			TemplatePathCache.Clear();
			ResourcePathCache.Clear();
		}
	}
}
