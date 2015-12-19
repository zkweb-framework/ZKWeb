using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZKWeb.Model {
	/// <summary>
	/// 控制器的接口
	/// 例子
	///		[ExportMany]
	///		class TestController : IController {
	///			[Action("index.html")]
	///			public string Index() { return "test index"; }
	///		}
	/// </summary>
	public interface IController {
	}

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
		/// <param name="path">路径，前面没有/时会自动添加</param>
		/// <param name="method">请求类型，默认是GET</param>
		public ActionAttribute(string path, string method = HttpMethods.GET) {
			Path = path.StartsWith("/") ? path : ("/" + path);
			Method = method;
		}
	}

	/// <summary>
	/// 常用的http的请求类型定义
	/// </summary>
	public static class HttpMethods {
		/// <summary>
		/// GET
		/// </summary>
		public const string GET = "GET";
		/// <summary>
		/// POST
		/// </summary>
		public const string POST = "POST";
		/// <summary>
		/// PUT
		/// </summary>
		public const string PUT = "PUT";
		/// <summary>
		/// DELETE
		/// </summary>
		public const string DELETE = "DELETE";
		/// <summary>
		/// PATCH
		/// </summary>
		public const string PATCH = "PATCH";
	}
}
