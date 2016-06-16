using System;
using System.Drawing;
using System.Drawing.Imaging;
using ZKWeb.Web;
using ZKWebStandard.Web;

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
