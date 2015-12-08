using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKWeb.Core.Model {
	/// <summary>
	/// 网站配置
	/// </summary>
	public class WebsiteConfig {
		/// <summary>
		/// 数据库的链接字符串
		/// </summary>
		public string ConnectionString { get; set; }
		/// <summary>
		/// 使用的插件列表
		/// </summary>
		public List<string> Plugins { get; set; }
	}
}
