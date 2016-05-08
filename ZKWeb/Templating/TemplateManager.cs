using DotLiquid.FileSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotLiquid;
using ZKWeb.Utils.Collections;
using System.IO;
using System.Collections;
using ZKWeb.Utils.Functions;
using DryIoc;
using System.Text.RegularExpressions;
using ZKWeb.Utils.Extensions;
using ZKWeb.Server;
using ZKWeb.Templating.TemplateTags;
using ZKWeb.Templating.TemplateFilters;

namespace ZKWeb.Templating {
	/// <summary>
	/// 模板管理器
	/// 当前使用的模板系统是
	///		DotLiquid http://dotliquidmarkup.org/
	/// </summary>
	public class TemplateManager {
		/// <summary>
		/// 设置描画模板时，是否显示完整的例外信息使用的键名
		/// </summary>
		public const string DisplayFullExceptionForTemplateKey = "DisplayFullExceptionForTemplate";

		/// <summary>
		/// 初始化
		/// </summary>
		public TemplateManager() {
			// 默认所有文本和对象经过html编码
			Template.RegisterValueTypeTransformer(typeof(string), s => HttpUtility.HtmlEncode(s));
			Template.RegisterValueTypeTransformer(typeof(object), s => HttpUtility.HtmlEncode(s));
			// 允许描画HtmlString
			Template.RegisterSafeType(typeof(HtmlString), s => s);
			// 初始化DotLiquid
			// 这里会添加所有默认标签和过滤器，这里不添加下面注册时不能覆盖
			Liquid.UseRubyDateFormat = !Liquid.UseRubyDateFormat;
			Liquid.UseRubyDateFormat = !Liquid.UseRubyDateFormat;
			// 修改正则表达式的缓存大小，默认缓存只有15
			Regex.CacheSize = 0xffff;
			// 设置是否显示完整的例外信息
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			Context.DisplayFullException = configManager.WebsiteConfig
				.Extra.GetOrDefault<bool?>(DisplayFullExceptionForTemplateKey) ?? true;
			// 注册自定义标签
			Template.RegisterTag<Area>("area");
			Template.RegisterTag<Fetch>("fetch");
			Template.RegisterTag<HtmlLang>("html_lang");
			Template.RegisterTag<RawHtml>("raw_html");
			// 注册自定义过滤器
			Template.RegisterFilter(typeof(Filters));
			// 设置使用的文件系统
			Template.FileSystem = Application.Ioc.Resolve<TemplateFileSystem>();
		}

		/// <summary>
		/// 描画指定的模板到数据流中
		/// </summary>
		/// <param name="path">模板路径</param>
		/// <param name="argument">传给模板的参数</param>
		/// <param name="stream">数据流</param>
		public virtual void RenderTemplate(string path, object argument, Stream stream) {
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
				// 这里不能调用Dispose，见http://stackoverflow.com/questions/2666888
				var writer = new StreamWriter(stream);
				writer.WriteLine($"template file {path} not found");
				writer.Flush();
				return;
			}
			// 使用模板描画到数据流中
			template.Render(stream, parameters);
		}

		/// <summary>
		/// 描画指定的模板到字符串
		/// </summary>
		/// <param name="path">模板路径</param>
		/// <param name="argument">传给模板的参数</param>
		public virtual string RenderTemplate(string path, object argument) {
			using (var stream = new MemoryStream()) {
				RenderTemplate(path, argument, stream);
				stream.Seek(0, SeekOrigin.Begin);
				var reader = new StreamReader(stream);
				return reader.ReadToEnd();
			}
		}
	}
}
