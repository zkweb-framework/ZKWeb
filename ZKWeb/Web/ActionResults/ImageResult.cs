using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using ZKWeb.Web.Interfaces;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// 图片结果
	/// </summary>
	public class ImageResult : IActionResult, IDisposable {
		/// <summary>
		/// 图片对象
		/// </summary>
		public Image Image { get; set; }
		/// <summary>
		/// 图片格式
		/// </summary>
		public ImageFormat Format { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="image">图片对象</param>
		/// <param name="format">图片格式，默认是Jpeg</param>
		public ImageResult(Image image, ImageFormat format = null) {
			Image = image;
			Format = format ?? ImageFormat.Jpeg;
		}

		/// <summary>
		/// 写入到http回应中
		/// </summary>
		/// <param name="response">http回应</param>
		public void WriteResponse(HttpResponse response) {
			if (Format == ImageFormat.Jpeg) {
				response.ContentType = "image/jpeg";
			} else if (Format == ImageFormat.Bmp) {
				response.ContentType = "image/bmp";
			} else if (Format == ImageFormat.Gif) {
				response.ContentType = "image/gif";
			} else if (Format == ImageFormat.Icon) {
				response.ContentType = "image/x-icon";
			} else if (Format == ImageFormat.Png) {
				response.ContentType = "image/png";
			}
			Image.Save(response.OutputStream, Format);
		}

		/// <summary>
		/// 清理资源
		/// </summary>
		public void Dispose() {
			Image.Dispose();
		}
	}
}
