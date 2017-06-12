using System.Collections.Generic;
using DotLiquid;
using System.IO;
using ZKWebStandard.Utils;
using System.Text.RegularExpressions;
using ZKWebStandard.Extensions;
using ZKWeb.Server;
using ZKWeb.Templating.TemplateTags;
using ZKWeb.Templating.TemplateFilters;
using ZKWebStandard.Collection;
using ZKWebStandard.Collections;

namespace ZKWeb.Templating {
	/// <summary>
	/// Template manager<br/>
	/// 模板管理器<br/>
	/// See: http://dotliquidmarkup.org/
	/// </summary>
	/// <example>
	/// <code language="cs">
	/// var templateManager = Application.Ioc.Resolve&lt;TemplateManager&gt;();
	/// var rendered = templateManager.RenderTemplate("index.html", null);
	/// </code>
	/// </example>
	public class TemplateManager {
		/// <summary>
		/// Internal cache size use to improve regex performance<br/>
		/// 正则表达式的内部缓存大小, 用于改进性能<br/>
		/// </summary>
		public const int RegexCacheSize = 0xffff;

		/// <summary>
		/// Create hash from object<br/>
		/// Please use this method to replace Hash.FromDictionary and Hash.FromAnonymousObject<br/>
		/// 创建Hash对象<br/>
		/// 请使用这个函数代替Hash.FromDictionary和Hash.FromAnonymousObject函数<br/>
		/// </summary>
		/// <param name="obj">Object</param>
		/// <returns></returns>
		public virtual Hash CreateHash(object obj) {
			if (obj == null) {
				return new Hash();
			} else if (obj is IDictionary<string, object>) {
				return Hash.FromDictionary((IDictionary<string, object>)obj);
			} else {
				return Hash.FromAnonymousObject(obj);
			}
		}

		/// <summary>
		/// Render template to stream<br/>
		/// 描画模板到数据流<br/>
		/// </summary>
		/// <param name="path">Template path</param>
		/// <param name="arguments">Template arguments</param>
		/// <param name="stream">Target stream</param>
		public virtual void RenderTemplate(string path, object arguments, Stream stream) {
			// Build template parameters
			var parameters = new RenderParameters();
			parameters.LocalVariables = CreateHash(arguments);
			// Find template, display error if not found
			var template = Template.FileSystem.ReadTemplateFile(null, path) as Template;
			if (template == null) {
				// Can't use using directive here, see http://stackoverflow.com/questions/2666888
				var writer = new StreamWriter(stream);
				writer.WriteLine($"template file {path} not found");
				writer.Flush();
				return;
			}
			// Render to stream
			template.Render(stream, parameters);
		}

		/// <summary>
		/// Render template to string<br/>
		/// 描画模板到字符串<br/>
		/// </summary>
		/// <param name="path">Template path</param>
		/// <param name="arguments">Template arguments</param>
		public virtual string RenderTemplate(string path, object arguments) {
			using (var stream = new MemoryStream()) {
				RenderTemplate(path, arguments, stream);
				stream.Seek(0, SeekOrigin.Begin);
				var reader = new StreamReader(stream);
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		internal protected virtual void Initialize() {
			// Force all string and object encode with html by default
			Template.RegisterValueTypeTransformer(typeof(string), s => HttpUtils.HtmlEncode(s));
			Template.RegisterValueTypeTransformer(typeof(object), s => HttpUtils.HtmlEncode(s));
			// Register safe type
			Template.RegisterSafeType(typeof(HtmlString), s => s);
			Template.RegisterSafeType(typeof(Pair<,>), new[] { "First", "Second" });
			Template.RegisterSafeType(typeof(ITreeNode<>), new[] { "Value", "Parent", "Childs" });
			// Call the static constructor here to add default tags and filters
			Liquid.UseRubyDateFormat = !Liquid.UseRubyDateFormat;
			Liquid.UseRubyDateFormat = !Liquid.UseRubyDateFormat;
			// Use bigger regex cache size
			Regex.CacheSize = RegexCacheSize;
			// Set if display full exception is allowed
			var configManager = Application.Ioc.Resolve<WebsiteConfigManager>();
			var extra = configManager.WebsiteConfig.Extra;
			Context.DisplayFullException = extra.GetOrDefault<bool?>(
				ExtraConfigKeys.DisplayFullExceptionForTemplate) ?? true;
			// Register custom tags
			Template.RegisterTag<Area>("area");
			Template.RegisterTag<Fetch>("fetch");
			Template.RegisterTag<HtmlLang>("html_lang");
			Template.RegisterTag<RawHtml>("raw_html");
			// Register custom filters
			Template.RegisterFilter(typeof(Filters));
			// Set template filesystem
			Template.FileSystem = Application.Ioc.Resolve<TemplateFileSystem>();
		}
	}
}
