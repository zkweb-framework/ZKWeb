using System.Reflection;
using System.Text;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// MethodInfo extensions
	/// </summary>
	public static class MethodInfoExtensions {
		/// <summary>
		/// Get full name of method
		/// Including type name and method name
		/// </summary>
		/// <param name="info">Method information</param>
		public static string GetFullName(this MethodInfo info) {
			var result = new StringBuilder();
			result.Append(info.DeclaringType.FullName).Append('.').Append(info.Name);
			return result.ToString();
		}
	}
}
