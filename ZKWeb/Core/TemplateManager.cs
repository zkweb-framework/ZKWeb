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
using ZKWeb.Model;
using ZKWeb.Core.TemplateFilters;

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
			Template.RegisterTag<HtmlLang>("html_lang");
			Template.RegisterTag<RawHtml>("raw_html");
			Template.RegisterTag<Widget>("widget");
			// 注册自定义过滤器
			Template.RegisterFilter(typeof(Filters));
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
	/// 使用PathManager.GetTemplateFullPath获取路径
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
		/// 从模板路径读取模板
		/// </summary>
		/// <param name="context">上下文</param>
		/// <param name="templateName">模板路径</param>
		/// <returns></returns>
		public object ReadTemplateFile(Context context, string templateName) {
			// 获取完整路径
			var pathManager = Application.Ioc.Resolve<PathManager>();
			var fullPath = pathManager.GetTemplateFullPath(templateName);
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
