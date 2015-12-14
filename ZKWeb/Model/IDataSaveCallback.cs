using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Core;

namespace ZKWeb.Model {
	/// <summary>
	/// 保存数据到数据库时的回调
	/// </summary>
	public interface IDataSaveCallback<T> {
		/// <summary>
		/// 保存前的处理
		/// </summary>
		/// <param name="context">使用的数据库上下文</param>
		/// <param name="data">数据</param>
		void BeforeSave(DatabaseContext context, T data);

		/// <summary>
		/// 保存后的处理
		/// </summary>
		/// <param name="context">使用的数据库上下文</param>
		/// <param name="data">数据</param>
		void AfterSave(DatabaseContext context, T data);
	}
}
