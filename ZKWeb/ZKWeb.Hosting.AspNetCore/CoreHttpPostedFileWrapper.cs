using Microsoft.AspNetCore.Http;
using System.IO;
using ZKWebStandard.Web;

namespace ZKWeb.Hosting.AspNetCore {
	/// <summary>
	/// Posted file wrapper for Asp.Net Core
	/// </summary>
	internal class CoreHttpPostedFileWrapper : IHttpPostedFile {
		/// <summary>
		/// Form file object
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
		/// Initialize
		/// </summary>
		/// <param name="coreFile">Form file object</param>
		public CoreHttpPostedFileWrapper(IFormFile coreFile) {
			CoreFile = coreFile;
		}
	}
}
