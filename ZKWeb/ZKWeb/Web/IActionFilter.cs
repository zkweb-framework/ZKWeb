using System;

namespace ZKWeb.Web {
	/// <summary>
	/// Action filter<br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	public interface IActionFilter {
		/// <summary>
		/// Return filtered action from original action<br/>
		/// <br/>
		/// </summary>
		/// <param name="action">Original action</param>
		/// <returns></returns>
		Func<IActionResult> Filter(Func<IActionResult> action);
	}
}
