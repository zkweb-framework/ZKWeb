using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Utils.Extensions {
	/// <summary>
	/// 成员信息的扩展函数
	/// </summary>
	public static class MemberInfoExtensions {
		/// <summary>
		/// 获取指定类型的属性，没有时返回null
		/// </summary>
		/// <typeparam name="TAttribute">属性类型</typeparam>
		/// <param name="info">成员信息</param>
		/// <returns></returns>
		public static TAttribute GetAttribute<TAttribute>(this MemberInfo info)
			where TAttribute : Attribute {
			return info.GetCustomAttributes(
				typeof(TAttribute), true).FirstOrDefault() as TAttribute;
		}
	}
}
