using System;
using System.Collections.Generic;
using System.Linq;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// IEnumerable extension methods<br/>
	/// 可枚举对象的扩展函数<br/>
	/// </summary>
	public static class IEnumerableExtensions {
		/// <summary>
		/// Concat object if it's not null<br/>
		/// 连接对象如果对象不等于null<br/>
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="elements">Elements</param>
		/// <param name="element">The object to concat</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var a = new[] { "a", "b", "c" };
		/// var b = a.ConcatIfNotNull("d");
		/// Assert.Equals(b.Count(), 4);
		/// Assert.IsTrueWith(b.Contains("d"), b);
		/// var c = a.ConcatIfNotNull(null);
		/// Assert.Equals(c.Count(), 3);
		/// Assert.IsTrueWith(!c.Contains(null), c);
		/// </code>
		/// </example>
		public static IEnumerable<T> ConcatIfNotNull<T>(
			this IEnumerable<T> elements, T element) {
			if (element != null) {
				return elements.Concat(new[] { element });
			}
			return elements;
		}

		/// <summary>
		/// Perform the given action to each element<br/>
		/// 对所有元素执行指定的操作<br/>
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="elements">Elements</param>
		/// <param name="action">The action</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var a = new[] { "a", "b", "c" };
		/// var b = new List&lt;string&gt;();
		/// a.ForEach(c =&gt; b.Add(c));
		/// Assert.Equals(b.Count, 3);
		/// Assert.IsTrueWith(a.SequenceEqual(b), new { a, b
		/// });
		/// </code>
		/// </example>
		public static void ForEach<T>(
			this IEnumerable<T> elements, Action<T> action) {
			foreach (var element in elements) {
				action(element);
			}
		}
	}
}
