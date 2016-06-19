using System;
using System.Collections.Generic;
using System.Linq;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// 序列的扩展函数
	/// </summary>
	public static class IEnumerableExtensions {
		/// <summary>
		/// 添加对象到指定的序列中，如果对象不等于null
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="elements">序列</param>
		/// <param name="element">添加的对象</param>
		/// <returns></returns>
		public static IEnumerable<T> ConcatIfNotNull<T>(
			this IEnumerable<T> elements, T element) {
			if (element != null) {
				return elements.Concat(new[] { element });
			}
			return elements;
		}

		/// <summary>
		/// 对序列中的每一个元素执行指定的函数
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="elements">序列</param>
		/// <param name="action">执行的函数</param>
		/// <returns></returns>
		public static void ForEach<T>(
			this IEnumerable<T> elements, Action<T> action) {
			foreach (var element in elements) {
				action(element);
			}
		}
	}
}
