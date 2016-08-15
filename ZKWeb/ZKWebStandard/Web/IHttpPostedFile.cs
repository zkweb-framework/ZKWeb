using System.IO;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Interface for posted file
	/// </summary>
	public interface IHttpPostedFile {
		/// <summary>
		/// Content type
		/// </summary>
		string ContentType { get; }
		/// <summary>
		/// File name
		/// </summary>
		string FileName { get; }
		/// <summary>
		/// File length
		/// </summary>
		long Length { get; }
		/// <summary>
		/// Open stream for reading file
		/// </summary>
		/// <returns></returns>
		Stream OpenReadStream();
	}
}
