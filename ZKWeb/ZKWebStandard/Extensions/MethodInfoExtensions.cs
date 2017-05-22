using System.Reflection;
using System.Text;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// MethodInfo extensions<br/>
	/// 函数信息的扩展函数<br/>
	/// </summary>
	public static class MethodInfoExtensions {
		/// <summary>
		/// Get full name of method<br/>
		/// Including type name and method name<br/>
		/// 获取函数的全名<br/>
		/// 包括类型的全名和函数名称<br/>
		/// </summary>
		/// <param name="info">Method information</param>
		/// <example>
		/// <code language="cs">
		/// var methodInfo = this.GetType().FastGetMethod("GetFullName");
		/// var fullname = methodInfo.GetFullName();
		/// </code>
		/// </example>
		public static string GetFullName(this MethodInfo info) {
			var result = new StringBuilder();
			result.Append(info.DeclaringType.FullName).Append('.').Append(info.Name);
			return result.ToString();
		}
	}
}
