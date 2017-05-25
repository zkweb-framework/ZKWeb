using System.IO;
using System.Web;
using ZKWebStandard.Web;

namespace ZKWeb.Hosting.AspNet {
	/// <summary>
	/// Http posted file wrapper for Asp.Net<br/>
	/// Asp.Net 提交文件的包装类<br/>
	/// </summary>
	internal class AspNetHttpPostedFileWrapper : IHttpPostedFile {
		/// <summary>
		/// Original posted file<br/>
		/// 原始的提交文件<br/>
		/// </summary>
		protected HttpPostedFile OriginalFile { get; set; }

		public string ContentType {
			get { return OriginalFile.ContentType; }
		}
		public string FileName {
			get { return OriginalFile.FileName; }
		}
		public long Length {
			get { return OriginalFile.ContentLength; }
		}

		public Stream OpenReadStream() {
			return OriginalFile.InputStream;
		}

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="originalFile">Original posted file</param>
		public AspNetHttpPostedFileWrapper(HttpPostedFile originalFile) {
			OriginalFile = originalFile;
		}
	}
}
