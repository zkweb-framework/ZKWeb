using System;

namespace ZKWeb.Web {
	/// <summary>
	/// Action filter
	/// </summary>
	public interface IActionFilter {
		/// <summary>
		/// Return filtered action from original action
		/// </summary>
		/// <param name="action">Original action</param>
		/// <returns></returns>
		Func<IActionResult> Filter(Func<IActionResult> action);
	}
}
