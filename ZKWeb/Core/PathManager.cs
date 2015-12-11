using DryIoc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Model;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Core {
	/// <summary>
	/// 路径管理器
	/// </summary>
	public class PathManager {
		/// <summary>
		/// 获取模板的完整路径
		/// 模板路径规则
		///	显式指定插件，这时不允许从其他插件或App_Data重载模板
		///		"所在插件:模板路径"
		///		例 "Common.Base:include/header.html"
		///		模板路径
		///			插件目录\templates\模板路径
		///		显式指定插件通常用于模板的继承
		/// 不指定插件，允许其他插件或App_Data重载模板
		///		"模板路径"
		///		例 "include/header.html"
		///		查找模板路径的顺序
		///			App_Data\templates\模板路径
		///			按载入顺序反向枚举插件
		///				插件目录\templates\模板路径
		///			同一模板路径可以在其他插件或在App_Data下重载
		/// 路径对应的文件不存在时返回null
		/// </summary>
		/// <param name="path">模板路径</param>
		/// <returns></returns>
		public string GetTemplateFullPath(string path) {
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
				var fullPath = PathUtils.SecureCombine(
					PathConfig.PluginsRootDirectory, explictPlugin, PathConfig.TemplateDirectoryName, path);
				return File.Exists(fullPath) ? fullPath : null;
			} else {
				// 不指定插件时，先从App_Data获取
				var fullPath = PathUtils.SecureCombine(
					PathConfig.AppDataDirectory, PathConfig.TemplateDirectoryName, path);
				if (File.Exists(fullPath)) {
					return fullPath;
				}
				// 从各个插件目录获取，按载入顺序反向枚举
				var pluginManager = Application.Ioc.Resolve<PluginManager>();
				foreach (var plugin in pluginManager.Plugins) {
					fullPath = PathUtils.SecureCombine(
						plugin.Directory, PathConfig.TemplateDirectoryName, path);
					if (File.Exists(fullPath)) {
						return fullPath;
					}
				}
				return null;
			}
		}
	}
}
