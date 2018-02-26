using System;

namespace ZKWeb.Web {
	/// <summary>
	/// Mark base action path of controller<br/>
	/// 标记控制器的基础路径<br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	/// <example>
	/// <code language="cs">
	/// [ActionBase("example")]
	/// public ExampleController : IController {
	///		public IActionResult Index() {
	///			return new PlainResult("abc");
	///		}
	///	}
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ActionBaseAttribute : Attribute {
		/// <summary>
		/// Path, use controller name if empty<br/>
		/// 基础路径, 为空则使用控制器名称<br/>
		/// </summary>
		public string PathBase { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public ActionBaseAttribute() { }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		public ActionBaseAttribute(string pathBase) {
			PathBase = pathBase;
		}
	}
}
