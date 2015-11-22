using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Utils.Extensions {
	/// <summary>
	/// DbSet的扩展函数
	/// </summary>
	public static class DbSetExtensions {
		/// <summary>
		/// 从数据表中删除符合条件的记录
		/// </summary>
		public static void RemoveWhere<T>(
			this DbSet<T> set, Expression<Func<T, bool>> expression)
			where T : class {
			set.RemoveRange(set.Where(expression));
		}
	}
}
