using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Utils.Extensions {
	/// <summary>
	/// 数据库例外的扩展函数
	/// </summary>
	public static class DataExceptionExtensions {
		/// <summary>
		/// 判断例外是否由指定的唯一索引冲突引起
		/// </summary>
		/// <param name="dataEx">例外</param>
		/// <param name="indexName">索引名称</param>
		/// <returns></returns>
		public static bool IsDuplicateFor(this DataException dataEx, string indexName) {
			Exception ex = dataEx;
			while (ex.InnerException != null) {
				ex = ex.InnerException;
			}
			return ex.Message?.Contains(indexName) ?? false;
		}

		/// <summary>
		/// 判断例外是否由外键依赖引起
		/// </summary>
		/// <param name="dataEx">例外</param>
		/// <returns></returns>
		public static bool IsForgenKeyError(this DataException dataEx) {
			Exception ex = dataEx;
			while (ex.InnerException != null) {
				ex = ex.InnerException;
			}
			return ex.Message?.Contains("violates foreign key") ?? false;
		}

		/// <summary>
		/// 判断例外是否由外键依赖引起并且消息包含指定字符串
		/// </summary>
		/// <param name="dataEx">例外</param>
		/// <param name="messageContains">判断是否包含的字符串</param>
		/// <returns></returns>
		public static bool IsForgenKeyError(this DataException dataEx, string messageContains) {
			Exception ex = dataEx;
			while (ex.InnerException != null) {
				ex = ex.InnerException;
			}
			return ((ex.Message?.Contains("violates foreign key") ?? false) &&
				(ex.Message?.Contains(messageContains) ?? false));
		}

		/// <summary>
		/// 判断例外是否由事务冲突引起的
		/// </summary>
		/// <param name="dataEx">例外</param>
		/// <returns></returns>
		public static bool IsConcurrentUpdate(this DataException dataEx) {
			Exception ex = dataEx;
			while (ex.InnerException != null) {
				ex = ex.InnerException;
			}
			return ex.Message?.Contains("concurrent update") ?? false;
		}
	}
}
