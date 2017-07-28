using System;
using System.Collections.Concurrent;
using System.FastReflection;
using System.Linq.Expressions;
using System.Reflection;
using ZKWebStandard.Collections;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Reflection utility functions<br/>
	/// 反射的工具函数<br/>
	/// </summary>
	public static class ReflectionUtils {
		private static readonly ConcurrentDictionary<Pair<Type, string>, object> SetterCache =
			new ConcurrentDictionary<Pair<Type, string>, object>();
		private static readonly ConcurrentDictionary<Pair<Type, string>, object> GetterCache =
			new ConcurrentDictionary<Pair<Type, string>, object>();
		private static readonly ConcurrentDictionary<Pair<MethodInfo, object>, MethodInvoker> InvokerCache =
			new ConcurrentDictionary<Pair<MethodInfo, object>, MethodInvoker>();

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
			var setter = SetterCache.GetOrAdd(Pair.Create(typeof(T), memberName), pair => {
				var objParam = Expression.Parameter(typeof(T), "obj");
				var memberParam = Expression.Parameter(typeof(M), "member");
				var memberExp = Expression.PropertyOrField(objParam, pair.Second);
				var body = Expression.Assign(memberExp, Expression.Convert(memberParam, memberExp.Type));
				return Expression.Lambda<Action<T, M>>(body, objParam, memberParam).Compile();
			});
			return (Action<T, M>)setter;
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
			var getter = GetterCache.GetOrAdd(Pair.Create(typeof(T), memberName), pair => {
				var objParam = Expression.Parameter(typeof(T), "obj");
				var body = Expression.Convert(Expression.PropertyOrField(objParam, pair.Second), typeof(M));
				return Expression.Lambda<Func<T, M>>(body, objParam).Compile();
			});
			return (Func<T, M>)getter;
		}

		/// <summary>
		/// Make invoker for method info<br/>
		/// 创建函数的调用器<br/>
		/// </summary>
		/// <example>
		/// <code language="cs">
		/// class TestData {
		///		int a;
		/// 	public int GetA() { return a; }
		/// 	public void SetA(int value) { a = value; }
		/// }
		/// 
		/// var getA = typeof(TestData).FastGetMethod(nameof(TestData.GetA));
		/// var setA = typeof(TestData).FastGetMethod(nameof(TestData.SetA));
		/// var getAInvoker = ReflectionUtils.MakeInvoker(getA);
		/// var setAInvoker = ReflectionUtils.MakeInvoker(setA);
		/// var data = new TestData();
		/// setAInvoker(data, new object[] { 123 });
		/// Console.WriteLine(getAInvoker(data, null));
		/// </code>
		/// </example>
		public static MethodInvoker MakeInvoker(MethodInfo methodInfo, Type genericType = null) {
			var invoker = InvokerCache.GetOrAdd(Pair.Create(methodInfo, (object)genericType), pair => {
				var methodInfoCopy = pair.First;
				if (pair.Second is Type type) {
					methodInfoCopy = pair.First.MakeGenericMethod((Type)pair.Second);
				}
				return ReflectionExtensions.MakeInvoker(methodInfoCopy);
			});
			return invoker;
		}

		/// <summary>
		/// Make invoker for method info<br/>
		/// 创建函数的调用器<br/>
		/// </summary>
		/// <seealso cref="MakeInvoker(MethodInfo, Type)"/>
		public static MethodInvoker MakeInvoker(MethodInfo methodInfo, Type[] genericTypes) {
			var key = (object)null;
			foreach (var type in genericTypes) {
				key = Pair.Create(type, key);
			}
			var invoker = InvokerCache.GetOrAdd(Pair.Create(methodInfo, key), pair => {
				var methodInfoCopy = pair.First.MakeGenericMethod(genericTypes);
				return ReflectionExtensions.MakeInvoker(methodInfoCopy);
			});
			return invoker;
		}

		/// <summary>
		/// Get generic arguments from type<br/>
		/// 获取类型继承的指定泛型类型的参数<br/>
		/// </summary>
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
