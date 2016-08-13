using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Linq expression extension methods
	/// </summary>
	public static class ExpressionExtensions {
		/// <summary>
		/// Get member information from lambda expression
		/// The expression should looks like: x => x.Name
		/// </summary>
		/// <param name="expression">Lambda expression</param>
		/// <returns></returns>
		public static MemberInfo GetMemberInfo(this LambdaExpression expression) {
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null) {
				throw new ArgumentException("GetMemberAttribute require body of expression is MemberExpression");
			}
			return memberExpression.Member;
		}

		/// <summary>
		/// Get member's attribute from lambda expression
		/// The expression should looks like: x => x.Name
		/// </summary>
		/// <typeparam name="TAttribute">Attribute type</typeparam>
		/// <param name="expression">Lambda expression</param>
		/// <returns></returns>
		public static TAttribute GetMemberAttribute<TAttribute>(this LambdaExpression expression)
			where TAttribute : Attribute {
			return expression.GetMemberInfo().GetCustomAttributes(
				typeof(TAttribute), true).FirstOrDefault() as TAttribute;
		}
	}
}
