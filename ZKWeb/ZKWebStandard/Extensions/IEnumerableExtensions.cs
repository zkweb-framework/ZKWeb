using System;
using System.Collections.Generic;
using System.Linq;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// IEnumerable extension methods<br/>
	/// <br/>
	/// </summary>
	public static class IEnumerableExtensions {
		/// <summary>
		/// Concat object if it's not null<br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="elements">Elements</param>
		/// <param name="element">The object to concat</param>
		/// <returns></returns>
		public static IEnumerable<T> ConcatIfNotNull<T>(
			this IEnumerable<T> elements, T element) {
			if (element != null) {
				return elements.Concat(new[] { element });
			}
			return elements;
		}

		/// <summary>
		/// Perform the given action to each element<br/>
		/// <br/>
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="elements">Elements</param>
		/// <param name="action">The action</param>
		/// <returns></returns>
		public static void ForEach<T>(
			this IEnumerable<T> elements, Action<T> action) {
			foreach (var element in elements) {
				action(element);
			}
		}
	}
}
