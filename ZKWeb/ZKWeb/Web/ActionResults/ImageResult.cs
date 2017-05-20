using System;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using ZKWebStandard.Web;

namespace ZKWeb.Web.ActionResults {
	/// <summary>
	/// Write image data to response<br/>
	/// 写入图片数据到回应<br/>
	/// </summary>
	/// <seealso cref="ControllerManager"/>
	/// <seealso cref="IController"/>
	/// <example>
	/// <code language="cs">
	/// public ExampleController : IController {
	///		[Action("example")]
	///		public IActionResult Example() {
	///			var image = Image.FromFile("c:\\1.jpg");
	///			return new ImageResult(image);
	///		}
	///	}
	/// </code>
	/// </example>
	public class ImageResult : IActionResult, IDisposable {
		/// <summary>
		/// Image object<br/>
		/// 图片对象<br/>
		/// </summary>
		public Image Image { get; set; }
		/// <summary>
		/// Image format<br/>
		/// 图片格式<br/>
		/// </summary>
		public ImageFormat Format { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="image">Image object</param>
		/// <param name="format">Image format, default is Jpeg</param>
		public ImageResult(Image image, ImageFormat format = null) {
			Image = image;
			Format = format ?? ImageFormat.Jpeg;
		}

		/// <summary><br/>
		/// Write image to http response<br/>
		/// 写入图片到Http回应
		/// </summary>
		/// <param name="response">Http回应</param>
		public void WriteResponse(IHttpResponse response) {
			// Set status code and content type
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
			// Write image to http response
			Image.Save(response.Body, Format);
			response.Body.Flush();
		}

		/// <summary>
		/// Clear resources<br/>
		/// 清理资源<br/>
		/// </summary>
		public void Dispose() {
			Image.Dispose();
		}
	}
}
