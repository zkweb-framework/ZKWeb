using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKWeb.Model {
	/// <summary>
	/// 控制器的基本类
	/// 如何使用这个类
	/// class HelloController : ControllerBase {
	///		[Get("/")]
	///		void Test(HttpApplication app) {
	///			app.Response.Write("hello world");
	///		}
	/// }
	/// </summary>
	public abstract class ControllerBase {
		/// <summary>
		/// 处理Http请求
		/// </summary>
		/// <param name="arg"></param>
		/// <param name="stop"></param>
		public void Handle(object arg, ref bool stop) {
			var app = (Application)arg;
			var path = app.Request.Path;
			var method = app.Request.HttpMethod;
			if (method == "GET") {

			} else if (method == "POST") {

			}
		}

		/// <summary>
		/// 标记函数可以处理指定路径的Get请求
		/// </summary>
		[AttributeUsage(AttributeTargets.Method)]
		public class GetAttribute : Attribute {
			public string Path { get; set; }
			public GetAttribute(string path) { Path = path; }
		}

		/// <summary>
		/// 标记函数可以处理指定路径的Post请求
		/// </summary>
		[AttributeUsage(AttributeTargets.Method)]
		public class PostAttribute : Attribute {
			public string Path { get; set; }
			public PostAttribute(string path) { Path = path; }
		}
	}
}
