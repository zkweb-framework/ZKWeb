using DryIoc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Core;

namespace ZKWeb.Model {
	/// <summary>
	/// 支持可视化编辑的模块
	/// </summary>
	public class DiyWidget {
		/// <summary>
		/// 模块信息
		/// </summary>
		public DiyWidgetInfo Info { get; set; }
		/// <summary>
		/// 模块参数
		/// </summary>
		public object Args { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="path">模块路径，注意不能带后缀</param>
		/// <param name="args">模块参数</param>
		public DiyWidget(string path, object args = null) {
			Info = DiyWidgetInfo.FromPath(path);
			Args = args;
		}
	}

	/// <summary>
	/// 支持可视化编辑的模块信息
	/// 由{模块路径}.widget文件反序列化生成
	/// </summary>
	public class DiyWidgetInfo {
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
		/// 从路径读取模块信息
		/// </summary>
		/// <param name="path">模块路径，注意不能带后缀</param>
		/// <returns></returns>
		public static DiyWidgetInfo FromPath(string path) {
			var pathManager = Application.Ioc.Resolve<PathManager>();
			var fullPath = pathManager.GetResourceFullPath(
				PathConfig.TemplateDirectoryName, path + InfoExtension);
			if (fullPath == null) {
				throw new FileNotFoundException($"widget {path} not exist");
			}
			var json = File.ReadAllText(fullPath);
			var widgetInfo = JsonConvert.DeserializeObject<DiyWidgetInfo>(json);
			widgetInfo.WidgetPath = path;
			widgetInfo.Name = widgetInfo.Name ?? Path.GetFileNameWithoutExtension(path);
			return widgetInfo;
		}
	}
}
