using System.Reflection;
using System.Text;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// MethodInfo extensions<br/>
	/// <br/>
	/// </summary>
	public static class MethodInfoExtensions {
		/// <summary>
		/// Get full name of method<br/>
		/// Including type name and method name<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="info">Method information</param>
		public static string GetFullName(this MethodInfo info) {
			var result = new StringBuilder();
			result.Append(info.DeclaringType.FullName).Append('.').Append(info.Name);
			return result.ToString();
		}
	}
}
