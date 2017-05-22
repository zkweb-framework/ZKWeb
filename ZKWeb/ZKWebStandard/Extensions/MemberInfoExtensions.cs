using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.FastReflection;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// MemberInfo extensions<br/>
	/// 成员信息的扩展函数<br/>
	/// </summary>
	public static class MemberInfoExtensions {
		/// <summary>
		/// Get specified type attribute<br/>
		/// Return null if not found<br/>
		/// Will not search inherited attributes<br/>
		/// 获取指定类型的属性<br/>
		/// 不存在时返回null<br/>
		/// 不搜索继承的属性<br/>
		/// </summary>
		/// <typeparam name="TAttribute">Attribute type</typeparam>
		/// <param name="info">Member infomation</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var info = typeof(TestData).FastGetProperty("TestProperty");
		/// var attribute = info.GetAttribute&lt;DescriptionAttribute&gt;();
		/// </code>
		/// </example>
		public static TAttribute GetAttribute<TAttribute>(this MemberInfo info)
			where TAttribute : Attribute {
			return info.GetAttributes<TAttribute>().FirstOrDefault();
		}

		/// <summary>
		/// Get specified type attributes<br/>
		/// 获取指定类型的属性<br/>
		/// </summary>
		/// <typeparam name="TAttribute">Attribute type</typeparam>
		/// <param name="info">Member infomation</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var info = typeof(TestData).FastGetProperty("TestProperty");
		/// var attributes = info.GetAttributes&lt;Attribute&gt;();
		/// </code>
		/// </example>
		public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this MemberInfo info)
			where TAttribute : Attribute {
			return info.FastGetCustomAttributes(typeof(TAttribute)).OfType<TAttribute>();
		}

		/// <summary>
		/// Get specified type attributes with inherit option<br/>
		/// 获取指定类型的属性, 可以指定是否搜索继承<br/>
		/// </summary>
		/// <typeparam name="TAttribute">Attribute type</typeparam>
		/// <param name="info">Member infomation</param>
		/// <param name="inherit">Should search override method or property's attributes</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var info = typeof(TestData).FastGetProperty("TestProperty");
		/// var attributes = info.GetAttributes&lt;Attribute&gt;(true);
		/// </code>
		/// </example>
		public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this MemberInfo info, bool inherit)
			where TAttribute : Attribute {
			return info.FastGetCustomAttributes(typeof(TAttribute), inherit).OfType<TAttribute>();
		}
	}
}
