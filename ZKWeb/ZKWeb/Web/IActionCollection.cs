using System;

namespace ZKWeb.Web {
	/// <summary>
	/// Collection used to manage actions<br/>
	/// 用于管理Action的集合<br/>
	/// </summary>
	public interface IActionCollection {
		/// <summary>
		/// Associate action with path and method<br/>
		/// 设置路径和方法关联的Action函数<br/>
		/// </summary>
		/// <param name="path">Normalized path</param>
		/// <param name="method">Http method</param>
		/// <param name="action">Action method</param>
		/// <param name="overrideExists">Allow override exists action</param>
		void Set(string path, string method, Func<IActionResult> action, bool overrideExists);

		/// <summary>
		/// Get action associated with path and method<br/>
		/// 获取路径和方法关联的Action函数<br/>
		/// </summary>
		/// <param name="path">Normalized path</param>
		/// <param name="method">Http method</param>
		/// <returns>Action method</returns>
		Func<IActionResult> Get(string path, string method);

		/// <summary>
		/// Remove action associated with path and method<br/>
		/// 删除路径和方法关联的Action函数<br/>
		/// </summary>
		/// <param name="path">Normalized path</param>
		/// <param name="method">Http method</param>
		/// <returns>true if removed or false if not found</returns>
		bool Remove(string path, string method);
	}
}
