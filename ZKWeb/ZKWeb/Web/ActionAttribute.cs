using System;

namespace ZKWeb.Web {
	/// <summary>
	/// Mark action method in controller<br/>
	/// 在控制器中标记Action函数<br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	/// <example>
	/// <code language="cs">
	/// public ExampleController : IController {
	///		[Action("example/index")]
	///		public IActionResult Index() {
	///			return new PlainResult("abc");
	///		}
	///	}
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class ActionAttribute : Attribute {
		/// <summary>
		/// Path, use method name if empty<br/>
		/// 路径, 为空则使用函数名<br/>
		/// </summary>
		public string Path { get; set; }
		/// <summary>
		/// Method, GET or POST or whatever<br/>
		/// Http方法, GET或者POST或者其他<br/>
		/// </summary>
		public string Method { get; set; }
		/// <summary>
		/// Allow override exist actions<br/>
		/// 是否允许覆盖现有的同路径的Action<br/>
		/// </summary>
		public bool OverrideExists { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public ActionAttribute() { }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public ActionAttribute(string path, string method = HttpMethods.GET) {
			Path = path;
			Method = method;
			OverrideExists = false;
		}
	}
}
