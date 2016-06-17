using System.IO;
using System.Web;
using ZKWebStandard.Web;

namespace ZKWeb.Web.Wrappers {
	/// <summary>
	/// 包装原始的提交文件
	/// </summary>
	internal class AspNetHttpPostedFileWrapper : IHttpPostedFile {
		/// <summary>
		/// 原始的提交文件
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
		/// 初始化
		/// </summary>
		/// <param name="originalFile">原始的提交文件</param>
		public AspNetHttpPostedFileWrapper(HttpPostedFile originalFile) {
			OriginalFile = originalFile;
		}
	}
}
