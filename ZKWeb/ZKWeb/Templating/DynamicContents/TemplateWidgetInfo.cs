using DotLiquid;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using ZKWeb.Storage;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template widget information<br/>
	/// Desirialize from {WidgetPath}.widget<br/>
	/// 模板模块的信息<br/>
	/// 从{模块路径}.widget文件反序列化得到<br/>
	/// </summary>
	/// <seealso cref="TemplateWidget"/>
	/// <seealso cref="TemplateAreaManager"/>
	public class TemplateWidgetInfo : ILiquidizable {
		/// <summary>
		/// Widget information file extension<br/>
		/// 模板模块信息的文件后缀名<br/>
		/// </summary>
		public const string InfoExtension = ".widget";
		/// <summary>
		/// Widget template file extension<br/>
		/// 模板的文件后缀名<br/>
		/// </summary>
		public const string HtmlExtension = ".html";
		/// <summary>
		/// Widget path, without extension<br/>
		/// 模块的路径, 不包含后缀名<br/>
		/// </summary>
		public string WidgetPath { get; set; }
		/// <summary>
		/// Widget name<br/>
		/// 模块的名称<br/>
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Widget description<br/>
		/// 模块的描述<br/>
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Cache time, in seconds<br/>
		/// 缓存时间, 单位是秒<br/>
		/// </summary>
		/// <example>15</example>
		public int CacheTime { get; set; }
		/// <summary>
		/// Cache isolation policies, separated by comma<br/>
		/// 缓存隔离策略, 使用逗号分割<br/>
		/// </summary>
		/// <example>Locale,Url</example>
		public string CacheBy { get; set; }
		/// <summary>
		/// Argument information list<br/>
		/// 模板模块的参数列表<br/>
		/// </summary>
		/// <example>
		/// <code>
		/// "Arguments": [
		///		{ "Name": "DisplayText", "Type": "TextBox" },
		///		{ "Name": "DisplayStyle", "Type": "DropdownList", "Provider": "ProviderName" },
		/// ]
		/// </code>
		/// </example>
		public IList<IDictionary<string, object>> Arguments { get; set; }
		/// <summary>
		/// Extra data<br/>
		/// 附加数据<br/>
		/// </summary>
		public IDictionary<string, object> Extra { get; set; }

		/// <summary>
		/// Read template widget information from path<br/>
		/// 从文件路径读取模板模块的信息<br/>
		/// </summary>
		/// <param name="path">Widget path, must without extension</param>
		/// <returns></returns>
		public static TemplateWidgetInfo FromPath(string path) {
			var fileStorage = Application.Ioc.Resolve<IFileStorage>();
			var templateFile = fileStorage.GetTemplateFile(path + InfoExtension);
			if (!templateFile.Exists) {
				throw new FileNotFoundException($"widget {path} not exist");
			}
			var json = templateFile.ReadAllText();
			var widgetInfo = JsonConvert.DeserializeObject<TemplateWidgetInfo>(json);
			widgetInfo.WidgetPath = path;
			widgetInfo.Name = widgetInfo.Name ?? Path.GetFileNameWithoutExtension(path);
			widgetInfo.Arguments = widgetInfo.Arguments ?? new List<IDictionary<string, object>>();
			widgetInfo.Extra = widgetInfo.Extra ?? new Dictionary<string, object>();
			return widgetInfo;
		}

		/// <summary>
		/// Support render to template<br/>
		/// 支持描画到模板<br/>
		/// </summary>
		/// <returns></returns>
		public object ToLiquid() {
			return new { WidgetPath, Name, Description, CacheTime, CacheBy, Arguments, Extra };
		}
	}
}
