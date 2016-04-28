using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.Extensions {
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
			return $"{info.DeclaringType.FullName}.{info.Name}";
		}
	}
}
