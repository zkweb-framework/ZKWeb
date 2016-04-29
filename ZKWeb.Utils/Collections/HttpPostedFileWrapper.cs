using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ZKWeb.Utils.Collections {
	/// <summary>
	/// http上传文件的包装类
	/// </summary>
	public class HttpPostedFileWrapper : HttpPostedFileBase {
		/// <summary>
		/// 包装的上传文件
		/// </summary>
		HttpPostedFile File { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="file">包装的上传文件</param>
		public HttpPostedFileWrapper(HttpPostedFile file) {
			File = file;
		}

		/// <summary>
		/// 获取内容长度
		/// </summary>
		public override int ContentLength { get { return File.ContentLength; } }
		
		/// <summary>
		/// 获取内容类型
		/// </summary>
		public override string ContentType { get { return File.ContentType; } }
		
		/// <summary>
		/// 获取文件名
		/// </summary>
		public override string FileName { get { return File.FileName; } }
		
		/// <summary>
		/// 获取流
		/// </summary>
		public override Stream InputStream { get { return File.InputStream; } }

		/// <summary>
		/// 保存文件
		/// </summary>
		/// <param name="filename">文件名</param>
		public override void SaveAs(string filename) {
			File.SaveAs(filename);
		}
	}
}
