using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKWeb.Database.Interfaces {
	/// <summary>
	/// 从数据库删除数据时的处理
	/// </summary>
	public interface IDataDeleteCallback<T> {
		/// <summary>
		/// 删除前的处理
		/// </summary>
		/// <param name="context">使用的数据库上下文</param>
		/// <param name="data">数据</param>
		void BeforeDelete(DatabaseContext context, T data);

		/// <summary>
		/// 删除后的处理
		/// </summary>
		/// <param name="context">使用的数据库上下文</param>
		/// <param name="data">数据</param>
		void AfterDelete(DatabaseContext context, T data);
	}
}
