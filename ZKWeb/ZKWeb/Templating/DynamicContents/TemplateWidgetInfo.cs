using DotLiquid;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using ZKWeb.Storage;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template widget information
	/// Desirialize from {WidgetPath}.widget
	/// </summary>
	public class TemplateWidgetInfo : ILiquidizable {
		/// <summary>
		/// Widget information file extension
		/// </summary>
		public const string InfoExtension = ".widget";
		/// <summary>
		/// Widget template file extension
		/// </summary>
		public const string HtmlExtension = ".html";
		/// <summary>
		/// Widget path, without extension
		/// </summary>
		public string WidgetPath { get; set; }
		/// <summary>
		/// Widget name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Widget description
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// Cache time, in seconds
		/// </summary>
		/// <example>15</example>
		public int CacheTime { get; set; }
		/// <summary>
		/// Cache isolation policies, separated by comma
		/// </summary>
		/// <example>Locale,Url</example>
		public string CacheBy { get; set; }
		/// <summary>
		/// Argument information list
		/// </summary>
		/// <example>
		/// "Arguments": [
		///		{ "Name": "DisplayText", "Type": "TextBoxField" },
		///		{ "Name": "DisplayStyle", "Type": "DropdownListField", "Provider": "TypeFullName" },
		/// ]
		/// </example>
		public IList<IDictionary<string, object>> Arguments { get; set; }
		/// <summary>
		/// Extra data
		/// </summary>
		public IDictionary<string, object> Extra { get; set; }

		/// <summary>
		/// Read template widget information from path
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
		/// Support render to template
		/// </summary>
		/// <returns></returns>
		public object ToLiquid() {
			return new { WidgetPath, Name, Description, CacheTime, CacheBy, Arguments, Extra };
		}
	}
}
