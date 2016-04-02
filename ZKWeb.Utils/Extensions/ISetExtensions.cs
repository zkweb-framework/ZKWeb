using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Utils.Extensions {
	/// <summary>
	/// 集合的扩展函数
	/// </summary>
	public static class ISetExtensions {
		/// <summary>
		/// 添加元素列表到集合中
		/// 返回添加成功的数量
		/// </summary>
		/// <typeparam name="T">元素类型</typeparam>
		/// <param name="set">集合</param>
		/// <param name="items">元素列表</param>
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
