using System.IO;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Interface for posted file<br/>
	/// Http提交文件的接口<br/>
	/// </summary>
	public interface IHttpPostedFile {
		/// <summary>
		/// Content type<br/>
		/// 内容类型<br/>
		/// </summary>
		string ContentType { get; }
		/// <summary>
		/// File name<br/>
		/// 文件名<br/>
		/// </summary>
		string FileName { get; }
		/// <summary>
		/// File length<br/>
		/// 文件长度<br/>
		/// </summary>
		long Length { get; }
		/// <summary>
		/// Open stream for reading file<br/>
		/// 打开读取文件的数据流<br/>
		/// </summary>
		/// <returns></returns>
		Stream OpenReadStream();
	}
}
