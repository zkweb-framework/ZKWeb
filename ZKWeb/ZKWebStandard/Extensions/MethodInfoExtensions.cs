using System.Reflection;
using System.Text;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// 函数信息的扩展函数
	/// </summary>
	public static class MethodInfoExtensions {
		/// <summary>
		/// 获取函数的完整名称
		/// 包含所在类的完整名称和函数名称
		/// </summary>
		/// <param name="info">函数信息</param>
		public static string GetFullName(this MethodInfo info) {
			var result = new StringBuilder();
			result.Append(info.DeclaringType.FullName).Append('.').Append(info.Name);
			return result.ToString();
		}
	}
}
