using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ZKWebStandard.Web {
	/// <summary>
	/// 提交的文件的接口
	/// </summary>
	public interface IHttpPostedFile {
		/// <summary>
		/// 文件类型
		/// </summary>
		string ContentType { get; }
		/// <summary>
		/// 文件名
		/// </summary>
		string FileName { get; }
		/// <summary>
		/// 长度
		/// </summary>
		long Length { get; }
		/// <summary>
		/// 打开读取文件的数据流
		/// </summary>
		/// <returns></returns>
		Stream OpenReadStream();
	}
}
