using System;

namespace ZKWeb.Web {
	/// <summary>
	/// Action filter attribute
	/// You need inherit it to provide the filter implementation
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public abstract class ActionFilterAttribute : Attribute, IActionFilter {
		/// <summary>
		/// Return filtered action from original action
		/// </summary>
		/// <param name="action">Original action</param>
		/// <returns></returns>
		public abstract Func<IActionResult> Filter(Func<IActionResult> action);
	}
}
