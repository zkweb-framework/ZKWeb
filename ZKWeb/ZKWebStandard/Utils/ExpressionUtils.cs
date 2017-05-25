using System;
using System.Linq.Expressions;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Expression utility functions<br/>
	/// Linq表达式的工具函数<br/>
	/// </summary>
	public static class ExpressionUtils {
		/// <summary>
		/// Make lambda expression that compare member to the given object<br/>
		/// Perform "data =&gt; data.{memberName} == {equalsTo}"<br/>
		/// 生成比较成员和指定对象是否相等的表达式<br/>
		/// 生成 "data =&gt; data.{memberName} == {equalsTo}"<br/>
		/// </summary>
		/// <typeparam name="TData">Data type</typeparam>
		/// <param name="memberName">Member name</param>
		/// <param name="equalsTo">Compare to</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var expression = ExpressionUtils.MakeMemberEqualiventExpression&lt;ICollection&lt;int&gt;&gt;("Count", 3);
		/// var method = expression.Compile();
		/// </code>
		/// </example>
		public static Expression<Func<TData, bool>> MakeMemberEqualiventExpression<TData>(
			string memberName, object equalsTo) {
			var dataParam = Expression.Parameter(typeof(TData), "data");
			var memberExp = Expression.Property(dataParam, memberName);
			var body = Expression.Equal(memberExp, Expression.Constant(equalsTo));
			return Expression.Lambda<Func<TData, bool>>(body, dataParam);
		}
	}
}
