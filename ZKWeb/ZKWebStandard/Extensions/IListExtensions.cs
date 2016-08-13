using System;
using System.Collections.Generic;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// IList Extension methods
	/// </summary>
	public static class IListExtensions {
		/// <summary>
		/// Find element index that match the given predicate
		/// Return -1 if not found
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="items">Elements</param>
		/// <param name="startIndex">Start position</param>
		/// <param name="match">The predicate</param>
		/// <returns></returns>
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
		/// Find element index that match the given predicate
		/// Return -1 if not found
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="items">Elements</param>
		/// <param name="match">The predicate</param>
		/// <returns></returns>
		public static int FindIndex<T>(
			this IList<T> items, Predicate<T> match) {
			return FindIndex(items, 0, match);
		}

		/// <summary>
		/// Find element index that match the given predicate from back to front
		/// Return -1 if not found
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="items">Elements</param>
		/// <param name="startIndex">Start position from back</param>
		/// <param name="match">The predicate</param>
		/// <returns></returns>
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
		/// Find element index that match the given predicate from back to front
		/// Return -1 if not found
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="items">Elements</param>
		/// <param name="match">The predicate</param>
		/// <returns></returns>
		public static int FindLastIndex<T>(
			this IList<T> items, Predicate<T> match) {
			return FindLastIndex(items, items.Count - 1, match);
		}

		/// <summary>
		/// Add element before the other element that match the given predicate
		/// If no other elements matched then add the element to front
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="items">Elements</param>
		/// <param name="before">The predicate</param>
		/// <param name="obj">Element want's to add</param>
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
		/// Add element after the other element that match the given predicate
		/// If no other elements matched then add the element to front
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="items">Elements</param>
		/// <param name="after">The predicate</param>
		/// <param name="obj">Element want's to add</param>
		public static void AddAfter<T>(
			this IList<T> items, Predicate<T> after, T obj) {
			var index = items.FindLastIndex(x => after(x));
			if (index < 0) {
				index = items.Count - 1;
			}
			items.Insert(index + 1, obj);
		}

		/// <summary>
		/// Batch add elements
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="list">Elements</param>
		/// <param name="items">Elements want's to add</param>
		/// <returns></returns>
		public static void AddRange<T>(this IList<T> list, IEnumerable<T> items) {
			foreach (var item in items) {
				list.Add(item);
			}
		}
	}
}
