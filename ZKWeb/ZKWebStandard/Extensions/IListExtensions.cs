using System;
using System.Collections.Generic;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// IList Extension methods<br/>
	/// 列表的扩展函数<br/>
	/// </summary>
	public static class IListExtensions {
		/// <summary>
		/// Find element index that match the given predicate<br/>
		/// Return -1 if not found<br/>
		/// 返回第一个符合指定条件的索引<br/>
		/// 如果无则返回-1<br/>
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="items">Elements</param>
		/// <param name="startIndex">Start position</param>
		/// <param name="match">The predicate</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// IList&lt;int&gt; list = new List&lt;int&gt;() { 1, 2, 4 };
		/// var index = list.FindIndex(2, x =&gt; x % 2 == 0); // 2
		/// </code>
		/// </example>
		public static int FindIndex<T>(
			this IList<T> items, int startIndex, Predicate<T> match) {
			if (startIndex < 0) {
				startIndex = 0;
			}
			for (int i = startIndex; i < items.Count; ++i) {
				if (match(items[i])) {
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Find element index that match the given predicate<br/>
		/// Return -1 if not found<br/>
		/// 返回第一个符合指定条件的索引<br/>
		/// 如果无则返回-1<br/>
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="items">Elements</param>
		/// <param name="match">The predicate</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// IList&lt;int&gt; list = new List&lt;int&gt;() { 1, 2, 4 };
		/// var index = list.FindIndex(x =&gt; x % 2 == 0); // 1
		/// </code>
		/// </example>
		public static int FindIndex<T>(
			this IList<T> items, Predicate<T> match) {
			return FindIndex(items, 0, match);
		}

		/// <summary>
		/// Find element index that match the given predicate from back to front<br/>
		/// Return -1 if not found<br/>
		/// 返回最后一个符合指定条件的索引<br/>
		/// 如果无则返回-1<br/>
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="items">Elements</param>
		/// <param name="startIndex">Start position from back</param>
		/// <param name="match">The predicate</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// IList&lt;int&gt; list = new List&lt;int&gt;() { 1, 2, 4 };
		/// var index = list.FindLastIndex(1, x => x % 2 == 0); // 1
		/// </code>
		/// </example>
		public static int FindLastIndex<T>(
			this IList<T> items, int startIndex, Predicate<T> match) {
			if (startIndex > items.Count - 1) {
				startIndex = items.Count - 1;
			}
			for (int i = startIndex; i >= 0; --i) {
				if (match(items[i])) {
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Find element index that match the given predicate from back to front<br/>
		/// Return -1 if not found<br/>
		/// 返回最后一个符合指定条件的索引<br/>
		/// 如果无则返回-1<br/>
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="items">Elements</param>
		/// <param name="match">The predicate</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// IList&lt;int&gt; list = new List&lt;int&gt;() { 1, 2, 4 };
		/// var index = list.FindLastIndex(x =&gt; x % 2 == 0); // 2
		/// </code>
		/// </example>
		public static int FindLastIndex<T>(
			this IList<T> items, Predicate<T> match) {
			return FindLastIndex(items, items.Count - 1, match);
		}

		/// <summary>
		/// Add element before the other element that match the given predicate<br/>
		/// If no other elements matched then add the element to front<br/>
		/// 在符合指定条件的元素前添加元素<br/>
		/// 如果无元素符合条件则添加到开头<br/>
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="items">Elements</param>
		/// <param name="before">The predicate</param>
		/// <param name="obj">Element want's to add</param>
		/// <example>
		/// <code language="cs">
		/// var list = new List&lt;int&gt;() { 1, 2, 4 };
		/// list.AddBefore(x =&gt; x == 4, 3); // 1, 2, 3, 4
		/// </code>
		/// </example>
		public static void AddBefore<T>(
			this IList<T> items, Predicate<T> before, T obj) {
			var a = new List<int>();
			a.FindIndex(_ => true);

			var index = items.FindIndex(x => before(x));
			if (index < 0) {
				index = 0;
			}
			items.Insert(index, obj);
		}

		/// <summary>
		/// Add element after the other element that match the given predicate<br/>
		/// If no other elements matched then add the element to front<br/>
		/// 在符合指定条件的元素后添加元素<br/>
		/// 如果无元素符合条件则添加到末尾<br/>
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="items">Elements</param>
		/// <param name="after">The predicate</param>
		/// <param name="obj">Element want's to add</param>
		/// <example>
		/// <code language="cs">
		/// var list = new List&lt;int&gt;() { 1, 2, 4 };
		/// list.AddAfter(x =&gt; x == 2, 3); // 1, 2, 3, 4
		/// </code>
		/// </example>
		public static void AddAfter<T>(
			this IList<T> items, Predicate<T> after, T obj) {
			var index = items.FindLastIndex(x => after(x));
			if (index < 0) {
				index = items.Count - 1;
			}
			items.Insert(index + 1, obj);
		}

		/// <summary>
		/// Batch add elements<br/>
		/// 批量添加元素<br/>
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="list">Elements</param>
		/// <param name="items">Elements want's to add</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// IList&lt;int&gt; list = new List&lt;int&gt;();
		/// list.AddRange(new[] { 1, 2, 3 });
		/// </code>
		/// </example>
		public static void AddRange<T>(this IList<T> list, IEnumerable<T> items) {
			foreach (var item in items) {
				list.Add(item);
			}
		}
	}
}
