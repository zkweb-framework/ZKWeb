using System.Collections.Generic;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// ISet extension methods<br/>
	/// <br/>
	/// </summary>
	public static class ISetExtensions {
		/// <summary>
		/// Batch add elements<br/>
		/// Return how many elements are not in the set before<br/>
		/// <br/>
		/// <br/>
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
