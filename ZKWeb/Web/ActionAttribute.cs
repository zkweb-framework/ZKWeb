using System;

namespace ZKWeb.Web.Abstractions {
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
		/// 是否重载现有的函数
		/// </summary>
		public bool OverrideExists { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="method">请求类型，默认是GET</param>
		public ActionAttribute(string path, string method = HttpMethods.GET) {
			Path = path;
			Method = method;
			OverrideExists = false;
		}
	}
}
