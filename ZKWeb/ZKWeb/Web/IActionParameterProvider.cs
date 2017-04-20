using System.Reflection;

namespace ZKWeb.Web {
	/// <summary>
	/// Interface used to get the action parameters
	/// For example:
	/// public class ExampleController : IController {
	///		public string ExampleAction(int x) {
	///			return x.ToString();
	///		}
	/// }
	/// Will call
	/// provider.GetParameter(
	///		"x",
	///		methodof(ExampleAction),
	///		parameterof(ExampleAction.x));
	/// </summary>
	public interface IActionParameterProvider {
		/// <summary>
		/// Get action parameter
		/// </summary>>
		/// <typeparam name="T">Parameter type</typeparam>
		/// <param name="name">Parameter name</param>
		/// <param name="method">Method information</param>
		/// <param name="parameterInfo">Parameter information</param>
		/// <returns></returns>
		T GetParameter<T>(string name, MethodInfo method, ParameterInfo parameterInfo);
	}
}
