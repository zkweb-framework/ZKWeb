using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.Extensions {
	/// <summary>
	/// 列表的扩展函数
	/// </summary>
	public static class IListExtensions {
		/// <summary>
		/// 查找符合条件的元素位置，找不到时返回-1
		/// </summary>
		/// <typeparam name="T">元素类型</typeparam>
		/// <param name="items">元素列表</param>
		/// <param name="startIndex">开始位置</param>
		/// <param name="match">匹配条件</param>
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
		/// 查找符合条件的元素位置，找不到时返回-1
		/// </summary>
		/// <typeparam name="T">元素类型</typeparam>
		/// <param name="items">元素列表</param>
		/// <param name="match">匹配条件</param>
		/// <returns></returns>
		public static int FindIndex<T>(
			this IList<T> items, Predicate<T> match) {
			return FindIndex(items, 0, match);
		}

		/// <summary>
		/// 从末尾开始查找符合条件的元素位置，找不到时返回-1
		/// </summary>
		/// <typeparam name="T">元素类型</typeparam>
		/// <param name="items">元素列表</param>
		/// <param name="startIndex">末尾的开始位置</param>
		/// <param name="match">匹配条件</param>
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
		/// 从末尾开始查找符合条件的元素位置，找不到时返回-1
		/// </summary>
		/// <typeparam name="T">元素类型</typeparam>
		/// <param name="items">元素列表</param>
		/// <param name="match">匹配条件</param>
		/// <returns></returns>
		public static int FindLastIndex<T>(
			this IList<T> items, Predicate<T> match) {
			return FindLastIndex(items, items.Count - 1, match);
		}

		/// <summary>
		/// 添加元素到指定元素的前面
		/// 如果没有则添加到最前面
		/// </summary>
		/// <param name="items">元素列表</param>
		/// <param name="before">添加到这个元素的前面</param>
		/// <param name="obj">添加的元素</param>
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
		/// 添加元素到指定元素的后面
		/// 如果没有则添加到最后面
		/// </summary>
		/// <param name="items">元素列表</param>
		/// <param name="after">添加到这个元素的后面</param>
		/// <param name="obj">添加的元素</param>
		public static void AddAfter<T>(
			this IList<T> items, Predicate<T> after, T obj) {
			var index = items.FindLastIndex(x => after(x));
			if (index < 0) {
				index = items.Count - 1;
			}
			items.Insert(index + 1, obj);
		}
	}
}
