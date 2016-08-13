using System;
using System.Drawing;
using System.Drawing.Imaging;
using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// Image result
	/// </summary>
	public class ImageResult : IActionResult, IDisposable {
		/// <summary>
		/// Image object
		/// </summary>
		public Image Image { get; set; }
		/// <summary>
		/// Image format
		/// </summary>
		public ImageFormat Format { get; set; }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="image">Image object</param>
		/// <param name="format">Image format, default is Jpeg</param>
		public ImageResult(Image image, ImageFormat format = null) {
			Image = image;
			Format = format ?? ImageFormat.Jpeg;
		}

		/// <summary>
		/// 写入图片到Http回应
		/// </summary>
		/// <param name="response">Http回应</param>
		public void WriteResponse(IHttpResponse response) {
			// 设置状态代码和内容类型
			response.StatusCode = 200;
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
			// 写入图片到回应
			Image.Save(response.Body, Format);
			response.Body.Flush();
		}

		/// <summary>
		/// 清理资源
		/// </summary>
		public void Dispose() {
			Image.Dispose();
		}
	}
}
