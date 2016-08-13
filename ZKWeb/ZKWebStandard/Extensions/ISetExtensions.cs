using System.Collections.Generic;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// ISet extension methods
	/// </summary>
	public static class ISetExtensions {
		/// <summary>
		/// Batch add elements
		/// Return how many elements are not in the set before
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="set">Element set</param>
		/// <param name="items">Elements want's to add</param>
		/// <returns></returns>
		public static long AddRange<T>(this ISet<T> set, IEnumerable<T> items) {
			long result = 0;
			foreach (var item in items) {
				result += set.Add(item) ? 1 : 0;
			}
			return result;
		}
	}
}
