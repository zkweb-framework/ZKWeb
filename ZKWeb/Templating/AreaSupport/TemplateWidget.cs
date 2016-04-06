using DryIoc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Server;

namespace ZKWeb.Templating.AreaSupport {
	/// <summary>
	/// 模板模块
	/// 用于在模板区域中显示部分内容
	/// </summary>
	public class TemplateWidget {
		/// <summary>
		/// 模块路径
		/// </summary>
		public string Path { get; set; }
		/// <summary>
		/// 模块参数
		/// </summary>
		public object Args { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="path">模块路径，注意不能带后缀</param>
		/// <param name="args">模块参数</param>
		public TemplateWidget(string path, object args = null) {
			Path = path;
			Args = args;
		}
	}
}
