using System.Reflection;

namespace ZKWeb.Web {
	/// <summary>
	/// Interface used to get the action parameters<br/>
	/// 用于获取Action参数的接口<br/>
	/// </summary>
	/// <example>
	/// For example:
	/// 例如:
	/// <code language="cs">
	/// public class ExampleController : IController {
	///		public string ExampleAction(int x) {
	///			return x.ToString();
	///		}
	/// }
	/// </code>
	/// 
	/// Will call:
	/// 会调用:
	/// <code language="cs">
	/// provider.GetParameter(
	///		"x",
	///		methodof(ExampleAction),
	///		parameterof(ExampleAction.x));
	///	</code>
	/// </example>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	public interface IActionParameterProvider {
		/// <summary>
		/// Get action parameter<br/>
		/// 获取Action参数<br/>
		/// </summary>>
		/// <typeparam name="T">Parameter type</typeparam>
		/// <param name="name">Parameter name</param>
		/// <param name="method">Method information</param>
		/// <param name="parameterInfo">Parameter information</param>
		/// <returns></returns>
		T GetParameter<T>(string name, MethodInfo method, ParameterInfo parameterInfo);
	}
}
