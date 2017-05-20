using System;

namespace ZKWeb.Web {
	/// <summary>
	/// Action filter attribute<br/>
	/// You need inherit it to provide the filter implementation<br/>
	/// Action过滤器的属性<br/>
	/// 你需要继承它来提供过滤的实现<br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	/// <example>
	/// <code language="cs">
	/// [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	/// public class RequireLoginAttribute : ActionFilterAttribute {
	///		public override Func&lt;IActionResult&gt; Filter(Func&lt;IActionResult&gt; action) {
	///			return () => {
	///				if (!LoginManager.CheckLogin()) {
	///					return new RedirectResult("/login");
	///				}
	///				return action();
	///			};
	///		}
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public abstract class ActionFilterAttribute : Attribute, IActionFilter {
		/// <summary>
		/// Return wrapped action<br/>
		/// 返回包装的Action函数<br/>
		/// </summary>
		/// <param name="action">Original action</param>
		/// <returns></returns>
		public abstract Func<IActionResult> Filter(Func<IActionResult> action);
	}
}
