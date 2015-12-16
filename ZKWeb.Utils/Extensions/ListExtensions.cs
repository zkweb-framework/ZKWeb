using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.Extensions {
	/// <summary>
	/// 列表的扩展函数
	/// </summary>
	public static class ListExtensions {
		/// <summary>
		/// 添加元素到指定元素的前面
		/// 如果没有则添加到最前面
		/// </summary>
		/// <param name="items">元素列表</param>
		/// <param name="before">添加到这个元素的前面</param>
		/// <param name="obj">添加的元素</param>
		public static void AddBefore<T>(
			this List<T> items, Predicate<T> before, T obj) {
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
		/// <param name="before">添加到这个元素的后面</param>
		/// <param name="obj">添加的元素</param>
		public static void AddAfter<T>(
			this List<T> items, Predicate<T> after, T obj) {
			var index = items.FindLastIndex(x => after(x));
			if (index < 0) {
				index = items.Count - 1;
			}
			items.Insert(index + 1, obj);
		}
	}
}
