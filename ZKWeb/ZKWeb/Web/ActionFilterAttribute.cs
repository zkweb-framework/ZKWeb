using System;

namespace ZKWeb.Web {
	/// <summary>
	/// Action filter attribute<br/>
	/// You need inherit it to provide the filter implementation<br/>
	/// <br/>
	/// <br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public abstract class ActionFilterAttribute : Attribute, IActionFilter {
		/// <summary>
		/// Return filtered action from original action<br/>
		/// <br/>
		/// </summary>
		/// <param name="action">Original action</param>
		/// <returns></returns>
		public abstract Func<IActionResult> Filter(Func<IActionResult> action);
	}
}
