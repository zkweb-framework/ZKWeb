using Microsoft.AspNetCore.Http;
using System.IO;
using ZKWebStandard.Web;

namespace ZKWeb.AspNetCore.Hosting {
	/// <summary>
	/// 包装AspNetCore的提交文件
	/// </summary>
	internal class CoreHttpPostedFileWrapper : IHttpPostedFile {
		/// <summary>
		/// AspNetCore的提交文件
		/// </summary>
		protected IFormFile CoreFile { get; set; }

		public string ContentType {
			get { return CoreFile.ContentType; }
		}
		public string FileName {
			get { return CoreFile.FileName; }
		}
		public long Length {
			get { return CoreFile.Length; }
		}

		public Stream OpenReadStream() {
			return CoreFile.OpenReadStream();
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="coreFile">AspNetCore的提交文件</param>
		public CoreHttpPostedFileWrapper(IFormFile coreFile) {
			CoreFile = coreFile;
		}
	}
}
