using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Reflection utility functions
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

		/// <summary>
		/// Get generic arguments from type
		/// </summary>
		/// <param name="type">The type</param>
		/// <param name="genericType">The generic type</param>
		/// <example>GetGenericArguments(typeof(MyList), typeof(IList[]))</example>
		/// <returns></returns>
		public static Type[] GetGenericArguments(Type type, Type genericType) {
			var typeInfo = type.GetTypeInfo();
			foreach (var interfaceType in typeInfo.GetInterfaces()) {
				var interfaceTypeInfo = interfaceType.GetTypeInfo();
				if (interfaceTypeInfo.IsGenericType &&
					interfaceTypeInfo.GetGenericTypeDefinition() == genericType) {
					return interfaceTypeInfo.GetGenericArguments();
				}
			}
			while (typeInfo != null) {
				if (typeInfo.IsGenericType &&
					typeInfo.GetGenericTypeDefinition() == genericType) {
					return typeInfo.GetGenericArguments();
				}
				typeInfo = typeInfo.BaseType?.GetTypeInfo();
			}
			return null;
		}
	}
}
