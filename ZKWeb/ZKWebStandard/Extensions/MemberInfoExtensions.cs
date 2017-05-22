using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.FastReflection;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// MemberInfo extensions<br/>
	/// <br/>
	/// </summary>
	public static class MemberInfoExtensions {
		/// <summary>
		/// Get specified type attribute<br/>
		/// Return null if not found<br/>
		/// Will not search inherited attributes<br/>
		/// <br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="TAttribute">Attribute type</typeparam>
		/// <param name="info">Member infomation</param>
		/// <returns></returns>
		public static TAttribute GetAttribute<TAttribute>(this MemberInfo info)
			where TAttribute : Attribute {
			return info.GetAttributes<TAttribute>().FirstOrDefault();
		}

		/// <summary>
		/// Get specified type attributes<br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="TAttribute">Attribute type</typeparam>
		/// <param name="info">Member infomation</param>
		/// <returns></returns>
		public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this MemberInfo info)
			where TAttribute : Attribute {
			return info.FastGetCustomAttributes(typeof(TAttribute)).OfType<TAttribute>();
		}

		/// <summary>
		/// Get specified type attributes with inherit option<br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="TAttribute">Attribute type</typeparam>
		/// <param name="info">Member infomation</param>
		/// <param name="inherit">Should search override method or property's attributes</param>
		/// <returns></returns>
		public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this MemberInfo info, bool inherit)
			where TAttribute : Attribute {
			return info.FastGetCustomAttributes(typeof(TAttribute), inherit).OfType<TAttribute>();
		}
	}
}
