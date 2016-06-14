using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Linq表达式的扩展函数
	/// </summary>
	public static class ExpressionExtensions {
		/// <summary>
		/// 获取成员表达式中的成员信息
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static MemberInfo GetMemberInfo(this LambdaExpression expression) {
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null) {
				throw new ArgumentException("GetMemberAttribute require body of expression is MemberExpression");
			}
			return memberExpression.Member;
		}

		/// <summary>
		/// 获取成员表达式中成员带的属性对象
		/// </summary>
		/// <typeparam name="TAttribute"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static TAttribute GetMemberAttribute<TAttribute>(this LambdaExpression expression)
			where TAttribute : Attribute {
			return expression.GetMemberInfo().GetCustomAttributes(
				typeof(TAttribute), true).FirstOrDefault() as TAttribute;
		}
	}
}
