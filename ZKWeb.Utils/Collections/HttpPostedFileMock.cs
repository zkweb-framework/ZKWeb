using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ZKWeb.Utils.Collections {
	/// <summary>
	/// HttpPostedFileBase的虚拟实现类
	/// </summary>
	public class HttpPostedFileMock : HttpPostedFileBase {
		/// <summary>
		/// 数据流
		/// </summary>
		Stream _Stream;
		/// <summary>
		/// 内容类型
		/// </summary>
		string _ContentType;
		/// <summary>
		/// 文件名
		/// </summary>
		string _Filename;

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="stream">数据流</param>
		/// <param name="contentType">内容类型</param>
		/// <param name="filename">文件名</param>
		public HttpPostedFileMock(Stream stream, string contentType, string filename) {
			_Stream = stream;
			_ContentType = contentType;
			_Filename = filename;
		}

		/// <summary>
		/// 获取内容长度
		/// </summary>
		public override int ContentLength { get { return (int)_Stream.Length; } }

		/// <summary>
		/// 获取内容类型
		/// </summary>
		public override string ContentType { get { return _ContentType; } }

		/// <summary>
		/// 获取文件名
		/// </summary>
		public override string FileName { get { return _Filename; } }

		/// <summary>
		/// 获取流
		/// </summary>
		public override Stream InputStream { get { return _Stream; } }

		/// <summary>
		/// 保存文件
		/// </summary>
		/// <param name="filename">文件名</param>
		public override void SaveAs(string filename) {
			_Stream.Seek(0, SeekOrigin.Begin);
			var buffer = new byte[1024];
			using (var fstream = File.OpenWrite(filename)) {
				while (true) {
					int readBytes = _Stream.Read(buffer, 0, 1024);
					if (readBytes == 0) {
						break;
					}
					fstream.Write(buffer, 0, readBytes);
				}
				fstream.Flush();
			}
		}
	}
}
