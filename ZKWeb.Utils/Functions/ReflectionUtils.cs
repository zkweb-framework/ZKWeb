using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.Functions {
	/// <summary>
	/// 反射工具类
	/// </summary>
	public static class ReflectionUtils {
		/// <summary>
		/// 生成属性或字段的设置函数
		/// 可以用于私有的属性或字段
		/// </summary>
		/// <typeparam name="T">对象的类型</typeparam>
		/// <typeparam name="M">属性或字段的类型</typeparam>
		/// <param name="memberName">属性或字段的名称</param>
		/// <returns></returns>
		public static Action<T, M> MakeSetter<T, M>(string memberName) {
			var objParam = Expression.Parameter(typeof(T), "obj");
			var memberParam = Expression.Parameter(typeof(M), "member");
			var memberExp = Expression.PropertyOrField(objParam, memberName);
			var body = Expression.Assign(memberExp, Expression.Convert(memberParam, memberExp.Type));
			return Expression.Lambda<Action<T, M>>(body, objParam, memberParam).Compile();
		}

		/// <summary>
		/// 生成属性或字段的获取函数
		/// 可以用于私有的属性或字段
		/// </summary>
		/// <typeparam name="T">对象的类型</typeparam>
		/// <typeparam name="M">属性或字段的类型</typeparam>
		/// <param name="memberName">属性或字段的名称</param>
		/// <returns></returns>
		public static Func<T, M> MakeGetter<T, M>(string memberName) {
			var objParam = Expression.Parameter(typeof(T), "obj");
			var body = Expression.Convert(Expression.PropertyOrField(objParam, memberName), typeof(M));
			return Expression.Lambda<Func<T, M>>(body, objParam).Compile();
		}
	}
}
