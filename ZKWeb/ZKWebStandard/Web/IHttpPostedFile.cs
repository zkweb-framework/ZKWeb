using System.IO;

namespace ZKWebStandard.Web {
	/// <summary>
	/// Interface for posted file<br/>
	/// <br/>
	/// </summary>
	public interface IHttpPostedFile {
		/// <summary>
		/// Content type<br/>
		/// <br/>
		/// </summary>
		string ContentType { get; }
		/// <summary>
		/// File name<br/>
		/// <br/>
		/// </summary>
		string FileName { get; }
		/// <summary>
		/// File length<br/>
		/// <br/>
		/// </summary>
		long Length { get; }
		/// <summary>
		/// Open stream for reading file<br/>
		/// <br/>
		/// </summary>
		/// <returns></returns>
		Stream OpenReadStream();
	}
}
