using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.Functions {
	/// <summary>
	/// Linq表达式的工具函数
	/// </summary>
	public static class ExpressionUtils {
		/// <summary>
		/// 构建判断成员是否等于指定值的表达式
		/// 相当于"data => data.{memberName} == {equalsTo}"
		/// </summary>
		/// <typeparam name="TData">数据类型</typeparam>
		/// <param name="memberName">成员名称</param>
		/// <param name="equalsTo">比较值</param>
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
