using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Reflection utility functions<br/>
	/// 反射的工具函数<br/>
	/// </summary>
	public static class ReflectionUtils {
		/// <summary>
		/// Make setter for member, member can be non public<br/>
		/// 获取成员的setter函数, 成员可以是非公开的<br/>
		/// </summary>
		/// <typeparam name="T">Data Type</typeparam>
		/// <typeparam name="M">Member Type</typeparam>
		/// <param name="memberName">Member name</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// class TestData {
		/// 	int a;
		/// 	public int GetA() { return a; }
		/// 	public void SetA(int value) { a = value; }
		/// }
		/// 
		/// var setter = ReflectionUtils.MakeSetter&lt;TestData, int&gt;("a");
		/// var data = new TestData();
		/// setter(data, 1);
		/// data.GetA() == 1
		/// </code>
		/// </example>
		public static Action<T, M> MakeSetter<T, M>(string memberName) {
			var objParam = Expression.Parameter(typeof(T), "obj");
			var memberParam = Expression.Parameter(typeof(M), "member");
			var memberExp = Expression.PropertyOrField(objParam, memberName);
			var body = Expression.Assign(memberExp, Expression.Convert(memberParam, memberExp.Type));
			return Expression.Lambda<Action<T, M>>(body, objParam, memberParam).Compile();
		}

		/// <summary>
		/// Make getter for member, member can be non public<br/>
		/// 获取成员的getter函数, 成员可以是非公开的<br/>
		/// </summary>
		/// <typeparam name="T">Data Type</typeparam>
		/// <typeparam name="M">Member Type</typeparam>
		/// <param name="memberName">Member name</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// class TestData {
		/// 	int a;
		/// 	public int GetA() { return a; }
		/// 	public void SetA(int value) { a = value; }
		/// }
		/// 
		/// var getter = ReflectionUtils.MakeGetter&lt;TestData, int&gt;("a");
		/// var data = new TestData();
		/// data.SetA(1);
		/// Assert.Equals(getter(data), 1);
		/// </code>
		/// </example>
		public static Func<T, M> MakeGetter<T, M>(string memberName) {
			var objParam = Expression.Parameter(typeof(T), "obj");
			var body = Expression.Convert(Expression.PropertyOrField(objParam, memberName), typeof(M));
			return Expression.Lambda<Func<T, M>>(body, objParam).Compile();
		}

		/// <summary>
		/// Get generic arguments from type<br/>
		/// 获取类型继承的指定泛型类型的参数<br/>
		/// </summary>
		/// <param name="type">The type</param>
		/// <param name="genericType">The generic type</param>
		/// <example>
		/// <code>
		/// class MyList : List&lt;int&gt; { }
		/// 
		/// var args = GetGenericArguments(typeof(MyList), typeof(IList&lt;&gt;)); // [ typeof(int) ]
		/// </code>
		/// </example>
		/// <returns></returns>
		public static Type[] GetGenericArguments(Type type, Type genericType) {
			foreach (var interfaceType in type.GetInterfaces()) {
				if (interfaceType.IsGenericType &&
					interfaceType.GetGenericTypeDefinition() == genericType) {
					return interfaceType.GetGenericArguments();
				}
			}
			while (type != null) {
				if (type.IsGenericType &&
					type.GetGenericTypeDefinition() == genericType) {
					return type.GetGenericArguments();
				}
				type = type.BaseType;
			}
			return null;
		}
	}
}
