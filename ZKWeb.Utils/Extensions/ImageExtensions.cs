using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
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
		/// <param name="image">图片对象</param>
		/// <param name="filename">保存路径，保存前会自动创建上级目录</param>
		/// <param name="quality">压缩质量</param>
		public static void SaveJpeg(this Image image, string filename, long quality) {
			Directory.CreateDirectory(Path.GetDirectoryName(filename));
			var encoder = ImageCodecInfo.GetImageEncoders().First(
				c => c.FormatID == ImageFormat.Jpeg.Guid);
			var parameters = new EncoderParameters();
			parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
			image.Save(filename, encoder, parameters);
		}

		/// <summary>
		/// 保存到Icon图标
		/// http://stackoverflow.com/questions/11434673/bitmap-save-to-save-an-icon-actually-saves-a-png
		/// </summary>
		/// <param name="image">图片对象</param>
		/// <param name="filename">保存路径，保存前会自动创建上级目录</param>
		public static void SaveIcon(this Image image, string filename) {
			Directory.CreateDirectory(Path.GetDirectoryName(filename));
			using (var stream = new FileStream(filename, FileMode.Create)) {
				// 图标头部 (ico, 1张图片)
				stream.Write(new byte[] { 0, 0, 1, 0, 1, 0 }, 0, 6);
				// 图片大小
				stream.WriteByte(checked((byte)image.Width));
				stream.WriteByte(checked((byte)image.Height));
				// 调色板数量
				stream.WriteByte(0);
				// 预留
				stream.WriteByte(0);
				// 颜色平面数量
				stream.Write(new byte[] { 0, 0 }, 0, 2);
				// 每个像素的bit数
				stream.Write(new byte[] { 32, 0 }, 0, 2);
				// 图片数据的大小，需要写入后确定
				stream.Write(new byte[] { 0, 0, 0, 0 }, 0, 4);
				// 图片数据的偏移值，这里固定22
				stream.Write(new byte[] { 22, 0, 0, 0 }, 0, 4);
				// 写入png数据
				image.Save(stream, ImageFormat.Png);
				// 写入图片数据的大小
				long imageSize = stream.Length - 22;
				stream.Seek(14, SeekOrigin.Begin);
				stream.WriteByte((byte)(imageSize));
				stream.WriteByte((byte)(imageSize >> 8));
				stream.WriteByte((byte)(imageSize >> 16));
				stream.WriteByte((byte)(imageSize >> 24));
			}
		}

		/// <summary>
		/// 保存图片，根据文件名自动识别格式
		/// 压缩质量仅在图片格式是jpeg时有效，其他格式时会忽略这个参数
		/// </summary>
		/// <param name="image">图片对象</param>
		/// <param name="filename">保存路径，保存前会自动创建上级目录</param>
		/// <param name="quality">压缩质量</param>
		public static void SaveAuto(this Image image, string filename, long quality) {
			Directory.CreateDirectory(Path.GetDirectoryName(filename));
			var extension = Path.GetExtension(filename).ToLower();
			if (extension == ".jpg" || extension == ".jpeg") {
				image.SaveJpeg(filename, quality);
			} else if (extension == ".bmp") {
				image.Save(filename, ImageFormat.Bmp);
			} else if (extension == ".gif") {
				image.Save(filename, ImageFormat.Gif);
			} else if (extension == ".ico") {
				image.SaveIcon(filename);
			} else if (extension == ".png") {
				image.Save(filename, ImageFormat.Png);
			} else if (extension == ".tiff") {
				image.Save(filename, ImageFormat.Tiff);
			} else if (extension == ".exif") {
				image.Save(filename, ImageFormat.Exif);
			} else {
				throw new NotSupportedException(
					string.Format("unsupport image extension {0}", extension));
			}
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
