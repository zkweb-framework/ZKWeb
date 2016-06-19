using Newtonsoft.Json;
using System.IO;
using ZKWeb.Server;

namespace ZKWeb.Templating.DynamicContents {
	/// <summary>
	/// 模板模块的信息
	/// 由{模块路径}.widget文件反序列化生成
	/// </summary>
	public class TemplateWidgetInfo {
		/// <summary>
		/// 模块信息的后缀名
		/// </summary>
		public const string InfoExtension = ".widget";
		/// <summary>
		/// 模块内容的后缀名
		/// </summary>
		public const string HtmlExtension = ".html";
		/// <summary>
		/// 模块路径，不带后缀
		/// </summary>
		public string WidgetPath { get; set; }
		/// <summary>
		/// 模块名称
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 缓存时间，单位是秒
		/// </summary>
		public int CacheTime { get; set; }
		/// <summary>
		/// 缓存隔离策略
		/// 可以不指定或指定一个或多个，指定多个时使用逗号分隔
		/// </summary>
		/// <example>Locale,Url</example>
		public string CacheBy { get; set; }

		/// <summary>
		/// 从路径读取模块信息
		/// </summary>
		/// <param name="path">模块路径，注意不能带后缀</param>
		/// <returns></returns>
		public static TemplateWidgetInfo FromPath(string path) {
			var pathManager = Application.Ioc.Resolve<PathManager>();
			var fullPath = pathManager.GetTemplateFullPath(path + InfoExtension);
			if (fullPath == null) {
				throw new FileNotFoundException($"widget {path} not exist");
			}
			var json = File.ReadAllText(fullPath);
			var widgetInfo = JsonConvert.DeserializeObject<TemplateWidgetInfo>(json);
			widgetInfo.WidgetPath = path;
			widgetInfo.Name = widgetInfo.Name ?? Path.GetFileNameWithoutExtension(path);
			return widgetInfo;
		}
	}
}
