using System;
using System.Linq.Expressions;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Expression utility functions<br/>
	/// <br/>
	/// </summary>
	public static class ExpressionUtils {
		/// <summary>
		/// Make lambda expression that compare member to the given object<br/>
		/// Perform "data => data.{memberName} == {equalsTo}"<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="TData">Data type</typeparam>
		/// <param name="memberName">Member name</param>
		/// <param name="equalsTo">Compare to</param>
		/// <returns></returns>
		public static Expression<Func<TData, bool>> MakeMemberEqualiventExpression<TData>(
			string memberName, object equalsTo) {
			var dataParam = Expression.Parameter(typeof(TData), "data");
			var memberExp = Expression.Property(dataParam, memberName);
			var body = Expression.Equal(memberExp, Expression.Constant(equalsTo));
			return Expression.Lambda<Func<TData, bool>>(body, dataParam);
		}
	}
}
