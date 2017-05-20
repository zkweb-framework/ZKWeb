using System;

namespace ZKWeb.Web {
	/// <summary>
	/// Action filter<br/>
	/// Action过滤器<br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	public interface IActionFilter {
		/// <summary>
		/// Return wrapped action
		/// 返回包装的Action函数<br/>
		/// </summary>
		/// <param name="action">Original action</param>
		/// <returns></returns>
		Func<IActionResult> Filter(Func<IActionResult> action);
	}
}
