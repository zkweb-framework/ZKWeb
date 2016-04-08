using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKWeb.Web.Interfaces {
	/// <summary>
	/// 标记函数可以处理指定路径的http请求
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class ActionAttribute : Attribute {
		/// <summary>
		/// 路径
		/// </summary>
		public string Path { get; set; }
		/// <summary>
		/// 请求类型
		/// </summary>
		public string Method { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="method">请求类型，默认是GET</param>
		public ActionAttribute(string path, string method = HttpMethods.GET) {
			Path = path;
			Method = method;
		}
	}
}
