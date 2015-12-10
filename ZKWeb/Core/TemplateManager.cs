using DotLiquid.FileSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotLiquid;
using ZKWeb.Utils.Collections;
using System.IO;
using System.Collections;
using ZKWeb.Core.TemplateTags;
using ZKWeb.Utils.Functions;
using DryIoc;

namespace ZKWeb.Core {
	/// <summary>
	/// 模板管理器
	/// 当前使用的模板系统是
	///		DotLiquid http://dotliquidmarkup.org/
	/// </summary>
	public class TemplateManager {
		/// <summary>
		/// 初始化
		/// </summary>
		public TemplateManager() {
			// 默认所有文本和对象经过html编码
			Template.RegisterValueTypeTransformer(typeof(string), s => HttpUtility.HtmlEncode(s));
			Template.RegisterValueTypeTransformer(typeof(object), s => HttpUtility.HtmlEncode(s));
			// 初始化DotLiquid
			// 这里会添加所有默认标签和过滤器，这里不添加下面注册时不能覆盖
			Liquid.UseRubyDateFormat = !Liquid.UseRubyDateFormat;
			Liquid.UseRubyDateFormat = !Liquid.UseRubyDateFormat;
			// 注册自定义标签
			Template.RegisterTag<Area>("area");
			Template.RegisterTag<DefaultWidgets>("default_widgets");
			Template.RegisterTag<RawHtml>("raw_html");
			Template.RegisterTag<IncludeWrapper>("include");
			// 设置使用的文件系统
			Template.FileSystem = new TemplateFileSystem();
		}

		/// <summary>
		/// 描画指定的模板到数据流中
		/// </summary>
		/// <param name="path">模板路径</param>
		/// <param name="argument">传给模板的参数</param>
		/// <param name="stream"></param>
		public void RenderTemplate(string path, object argument, Stream stream) {
			// 构建模板的参数
			var parameters = new RenderParameters();
			if (argument is IDictionary<string, object>) {
				parameters.LocalVariables = Hash.FromDictionary((IDictionary<string, object>)argument);
			} else {
				parameters.LocalVariables = Hash.FromAnonymousObject(argument);
			}
			// 查找模板，找不到时写入错误信息
			var template = Template.FileSystem.ReadTemplateFile(null, path) as Template;
			if (template == null) {
				using (var writer = new StreamWriter(stream)) {
					writer.WriteLine($"template file {path} not found");
					writer.Flush();
				}
				return;
			}
			// 使用模板描画到数据流中
			template.Render(stream, parameters);
		}
	}

	/// <summary>
	/// 模板系统使用的文件系统
	/// 模板路径规则
	///	显式指定插件，这时不允许从其他插件或App_Data重载模板
	///		"所在插件:模板路径"
	///		例 "Common.Base:include/header.html"
	///		模板路径
	///			App_Code\插件目录\templates\模板路径
	///		显式指定插件通常用于模板的继承
	/// 不指定插件，允许其他插件或App_Data重载模板
	///		"模板路径"
	///		例 "include/header.html"
	///		查找模板路径的顺序
	///			App_Data\templates\模板路径
	///			按载入顺序反向枚举插件
	///				App_Code\插件目录\templates\模板路径
	///			同一模板路径可以在其他插件或在App_Data下重载
	/// </summary>
	public class TemplateFileSystem : IFileSystem {
		/// <summary>
		/// 缓存时间
		/// 这里的缓存时间是防止内存占用过多而设置的
		/// 无限期缓存也可以正常使用
		/// </summary>
		public TimeSpan CacheTime { get; set; } = TimeSpan.FromMinutes(15);
		/// <summary>
		/// 模板的缓存
		/// { 模板的绝对路径: (文件修改时间, 模板对象) }
		/// </summary>
		private MemoryCache<string, Tuple<DateTime, Template>> TemplateCache { get; set; } =
			new MemoryCache<string, Tuple<DateTime, Template>>();

		/// <summary>
		/// 获取模板的完整路径
		/// 请查看这个类的注释中的"模板路径规则"
		/// 路径对应的文件不存在时返回null
		/// </summary>
		/// <param name="path">模板路径</param>
		/// <returns></returns>
		public static string GetTemplateFullPath(string path) {
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

		/// <summary>
		/// 从模板路径读取模板
		/// </summary>
		/// <param name="context">上下文</param>
		/// <param name="templateName">模板路径</param>
		/// <returns></returns>
		public object ReadTemplateFile(Context context, string templateName) {
			// 获取完整路径
			var fullPath = GetTemplateFullPath(templateName);
			if (fullPath == null) {
				return null;
			}
			// 从缓存中获取
			var lastWriteTime = File.GetLastWriteTimeUtc(fullPath);
			var cache = TemplateCache.GetOrDefault(fullPath);
			if (cache != null && cache.Item1 == lastWriteTime) {
				return cache.Item2;
			}
			// 重新读取并设置到缓存中
			var sources = File.ReadAllText(fullPath);
			var template = Template.Parse(sources);
			TemplateCache.Put(fullPath, Tuple.Create(lastWriteTime, template), CacheTime);
			return template;
		}
	}
}
