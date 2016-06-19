using System.IO;
using System.Net.Http;
using ZKWebStandard.Web;

namespace ZKWeb.Owin.Hosting {
	/// <summary>
	/// 包装Owin的提交文件
	/// </summary>
	internal class OwinHttpPostedFileWrapper : IHttpPostedFile {
		/// <summary>
		/// Owin的提交文件
		/// </summary>
		protected HttpContent OwinFile { get; set; }

		public string ContentType {
			get { return OwinFile.Headers.ContentType.ToString(); }
		}
		public string FileName {
			get { return OwinFile.Headers.ContentDisposition.FileName.Trim('"'); }
		}
		public long Length {
			get { return OwinFile.Headers.ContentLength ?? 0; }
		}

		public Stream OpenReadStream() {
			return OwinFile.ReadAsStreamAsync().Result;
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="owinFile">Owin的提交文件</param>
		public OwinHttpPostedFileWrapper(HttpContent owinFile) {
			OwinFile = owinFile;
		}
	}
}
