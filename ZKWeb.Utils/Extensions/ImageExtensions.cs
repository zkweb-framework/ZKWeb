using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Utils.Extensions {
	/// <summary>
	/// 图片的扩展函数
	/// </summary>
	public static class ImageExtensions {
		/// <summary>
		/// 缩放图片
		/// </summary>
		/// <param name="image">原图片</param>
		/// <param name="width">宽度</param>
		/// <param name="height">高度</param>
		/// <param name="mode">缩放模式</param>
		/// <param name="background">背景颜色，默认是透明</param>
		/// <returns></returns>
		public static Image Resize(this Image image,
			int width, int height, ImageResizeMode mode, Color? background = null) {
			var src = new Rectangle(0, 0, image.Width, image.Height);
			var dst = new Rectangle(0, 0, width, height);
			// 根据模式调整缩放到的大小和位置
			if (mode == ImageResizeMode.Fixed) {
			} else if (mode == ImageResizeMode.ByWidth) {
				height = (int)((decimal)src.Height / src.Width * dst.Width);
				dst.Height = height;
			} else if (mode == ImageResizeMode.ByHeight) {
				width = (int)((decimal)src.Width / src.Height * dst.Height);
				dst.Width = width;
			} else if (mode == ImageResizeMode.Cut) {
				if ((decimal)src.Width / src.Height > (decimal)dst.Width / dst.Height) {
					src.Width = (int)((decimal)dst.Width / dst.Height * src.Height);
					src.X = (image.Width - src.Width) / 2; // 切除原图片左右
				} else {
					src.Height = (int)((decimal)dst.Height / dst.Width * src.Width);
					src.Y = (image.Height - src.Height) / 2; // 切除原图片上下
				}
			} else if (mode == ImageResizeMode.Padding) {
				if ((decimal)src.Width / src.Height > (decimal)dst.Width / dst.Height) {
					dst.Height = (int)((decimal)src.Height / src.Width * dst.Width);
					dst.Y = (height - dst.Height) / 2; // 扩展原图片左右
				} else {
					dst.Width = (int)((decimal)src.Width / src.Height * dst.Height);
					dst.X = (width - dst.Width) / 2; // 扩展原图片上下
				}
			}
			// 缩放到新图片上
			var newImage = new Bitmap(width, height);
			using (var graphics = Graphics.FromImage(newImage)) {
				// 设置高质量缩放
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				// 设置背景色
				graphics.Clear(background ?? Color.Transparent);
				// 在新图片上描画原图片
				graphics.DrawImage(image, dst, src, GraphicsUnit.Pixel);
			}
			return newImage;
		}

		/// <summary>
		/// 保存到Jpeg图片，且可以设置压缩质量
		/// </summary>
		/// <param name="image"></param>
		/// <param name="filename">保存路径</param>
		/// <param name="quality">压缩质量</param>
		public static void SaveJpeg(this Image image, string filename, long quality) {
			var encoder = ImageCodecInfo.GetImageEncoders().First(
				c => c.FormatID == ImageFormat.Jpeg.Guid);
			var parameters = new EncoderParameters();
			parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
			image.Save(filename, encoder, parameters);
		}
	}

	/// <summary>
	/// 图片缩放模式
	/// </summary>
	public enum ImageResizeMode {
		/// <summary>
		/// 缩放到指定的尺寸，允许变形
		/// </summary>
		Fixed,
		/// <summary>
		/// 固定宽度缩放，高度按比例计算
		/// </summary>
		ByWidth,
		/// <summary>
		/// 固定高度缩放，宽度按比例计算
		/// </summary>
		ByHeight,
		/// <summary>
		/// 固定宽度和高度缩放，对不符合比例的部分进行切除
		/// </summary>
		Cut,
		/// <summary>
		/// 固定宽度和高度缩放，对不符合比例的部分进行扩展
		/// </summary>
		Padding
	}
}