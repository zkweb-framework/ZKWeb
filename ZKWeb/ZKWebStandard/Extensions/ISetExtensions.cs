using System.Collections.Generic;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// ISet extension methods<br/>
	/// 集合的扩展函数<br/>
	/// </summary>
	public static class ISetExtensions {
		/// <summary>
		/// Batch add elements<br/>
		/// Return how many elements are not in the set before<br/>
		/// 批量添加元素<br/>
		/// 返回有多少个元素之前不在集合中<br/>
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="set">Element set</param>
		/// <param name="items">Elements want's to add</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// set = new SortedSet&lt;int&gt;();
		/// set.AddRange(new[] { 1, 2, 3 });
		/// </code>
		/// </example>
		public static long AddRange<T>(this ISet<T> set, IEnumerable<T> items) {
			long result = 0;
			foreach (var item in items) {
				result += set.Add(item) ? 1 : 0;
			}
			return result;
		}
	}
}
