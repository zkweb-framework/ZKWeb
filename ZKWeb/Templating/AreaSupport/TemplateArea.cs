using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Server;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Templating.AreaSupport {
	/// <summary>
	/// 模板区域
	/// </summary>
	public class TemplateArea {
		/// <summary>
		/// 区域Id
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// 默认的模块列表
		/// </summary>
		public IList<TemplateWidget> DefaultWidgets { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="id"></param>
		public TemplateArea(string id) {
			Id = id;
			DefaultWidgets = new List<TemplateWidget>();
		}
	}
}
