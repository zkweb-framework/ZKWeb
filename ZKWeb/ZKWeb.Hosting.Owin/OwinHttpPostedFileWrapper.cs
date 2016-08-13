using System.IO;
using System.Net.Http;
using ZKWebStandard.Web;

namespace ZKWeb.Hosting.Owin {
	/// <summary>
	/// Http poested file wrapper for Owin
	/// </summary>
	internal class OwinHttpPostedFileWrapper : IHttpPostedFile {
		/// <summary>
		/// Http content object
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
		/// Initialize
		/// </summary>
		/// <param name="owinFile">Http content object</param>
		public OwinHttpPostedFileWrapper(HttpContent owinFile) {
			OwinFile = owinFile;
		}
	}
}
