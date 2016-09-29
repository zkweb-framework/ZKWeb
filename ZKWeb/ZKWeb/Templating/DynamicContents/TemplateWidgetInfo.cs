using Newtonsoft.Json;
using System.IO;
using ZKWeb.Server;
using ZKWeb.Storage;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// Template widget information
	/// Desirialize from {WidgetPath}.widget
	/// </summary>
	public class TemplateWidgetInfo {
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
		/// Cache time, in seconds
		/// </summary>
		public int CacheTime { get; set; }
		/// <summary>
		/// Cache isolation policies, separated by comma
		/// </summary>
		/// <example>Locale,Url</example>
		public string CacheBy { get; set; }

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
			return widgetInfo;
		}
	}
}
