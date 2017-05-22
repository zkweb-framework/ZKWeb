using System;
using System.DrawingCore;
using System.DrawingCore.Drawing2D;
using System.DrawingCore.Imaging;
using System.IO;
using System.Linq;
using ZKWebStandard.Utils;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Image extension methods<br/>
	/// 图片的扩展函数<br/>
	/// </summary>
	public static class ImageExtensions {
		/// <summary>
		/// Resize image<br/>
		/// 改变图片大小<br/>
		/// </summary>
		/// <param name="image">Original image</param>
		/// <param name="width">Width</param>
		/// <param name="height">Height</param>
		/// <param name="mode">Resize mode</param>
		/// <param name="background">Background, default is transparent</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var newImage = oldImage.Resize(100, 100, ImageResizeMode.Fixed);
		/// </code>
		/// </example>
		public static Image Resize(this Image image,
			int width, int height, ImageResizeMode mode, Color? background = null) {
			var src = new Rectangle(0, 0, image.Width, image.Height);
			var dst = new Rectangle(0, 0, width, height);
			// Calculate destination rectangle by resize mode
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
					src.X = (image.Width - src.Width) / 2; // Cut left and right
				} else {
					src.Height = (int)((decimal)dst.Height / dst.Width * src.Width);
					src.Y = (image.Height - src.Height) / 2; // Cut top and bottom
				}
			} else if (mode == ImageResizeMode.Padding) {
				if ((decimal)src.Width / src.Height > (decimal)dst.Width / dst.Height) {
					dst.Height = (int)((decimal)src.Height / src.Width * dst.Width);
					dst.Y = (height - dst.Height) / 2; // Padding left and right
				} else {
					dst.Width = (int)((decimal)src.Width / src.Height * dst.Height);
					dst.X = (width - dst.Width) / 2; // Padding top and bottom
				}
			}
			// Draw new image
			var newImage = new Bitmap(width, height);
			using (var graphics = Graphics.FromImage(newImage)) {
				// Set smoothing mode
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				// Set background color
				graphics.Clear(background ?? Color.Transparent);
				// Render original image with the calculated rectangle
				graphics.DrawImage(image, dst, src, GraphicsUnit.Pixel);
			}
			return newImage;
		}

		/// <summary>
		/// Save to jpeg file<br/>
		/// 保存到jpeg文件<br/>
		/// </summary>
		/// <param name="image">Image object</param>
		/// <param name="filename">File path, will automatic create parent directories</param>
		/// <param name="quality">Compress quality, 1~100</param>
		[Obsolete("Please use SaveAuto")]
		public static void SaveJpeg(this Image image, string filename, long quality) {
			PathUtils.EnsureParentDirectory(filename);
			using (var stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write)) {
				image.SaveJpeg(stream, quality);
			}
		}

		/// <summary>
		/// Save to jpeg<br/>
		/// 保存到jpeg<br/>
		/// </summary>
		/// <param name="image">Image object</param>
		/// <param name="stream">Stream object</param>
		/// <param name="quality">Compress quality, 1~100</param>
		/// <example>
		/// <code language="cs">
		/// using (var stream = new MemoryStream()) {
		///		image.SaveJpeg(stream, 90);
		/// }
		/// </code>
		/// </example>
		private static void SaveJpeg(this Image image, Stream stream, long quality) {
			var encoder = ImageCodecInfo.GetImageEncoders().First(
				c => c.FormatID == ImageFormat.Jpeg.Guid);
			var parameters = new EncoderParameters();
			parameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
			image.Save(stream, encoder, parameters);
		}

		/// <summary>
		/// Save to icon file<br/>
		/// 保存到图标文件<br/>
		/// </summary>
		/// <param name="image">Image object</param>
		/// <param name="filename">File path, will automatic create parent directories</param>
		[Obsolete("Please use SaveAuto")]
		public static void SaveIcon(this Image image, string filename) {
			PathUtils.EnsureParentDirectory(filename);
			using (var stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write)) {
				image.SaveIcon(stream);
			}
		}

		/// <summary>
		/// Save to icon, see<br/>
		/// 保存到图标, 请查看<br/>
		/// http://stackoverflow.com/questions/11434673/bitmap-save-to-save-an-icon-actually-saves-a-png
		/// </summary>
		/// <param name="image">Image object</param>
		/// <param name="stream">Stream object</param>
		/// <example>
		/// <code language="cs">
		/// using (var stream = new MemoryStream()) {
		///		image.SaveIcon(stream);
		/// }
		/// </code>
		/// </example>
		private static void SaveIcon(this Image image, Stream stream) {
			// Header (ico, 1 photo)
			stream.Write(new byte[] { 0, 0, 1, 0, 1, 0 }, 0, 6);
			// Size
			stream.WriteByte(checked((byte)image.Width));
			stream.WriteByte(checked((byte)image.Height));
			// No palette
			stream.WriteByte(0);
			// Reserved
			stream.WriteByte(0);
			// No color planes
			stream.Write(new byte[] { 0, 0 }, 0, 2);
			// 32 bpp
			stream.Write(new byte[] { 32, 0 }, 0, 2);
			// Image data length, set later
			stream.Write(new byte[] { 0, 0, 0, 0 }, 0, 4);
			// Image data offset, fixed 22 here
			stream.Write(new byte[] { 22, 0, 0, 0 }, 0, 4);
			// Write png data
			image.Save(stream, ImageFormat.Png);
			// Write image data length
			long imageSize = stream.Length - 22;
			stream.Seek(14, SeekOrigin.Begin);
			stream.WriteByte((byte)(imageSize));
			stream.WriteByte((byte)(imageSize >> 8));
			stream.WriteByte((byte)(imageSize >> 16));
			stream.WriteByte((byte)(imageSize >> 24));
		}

		/// <summary>
		/// Save image by it's file extension<br/>
		/// Quality parameter only available for jpeg<br/>
		/// 根据文件后缀保存图片<br/>
		/// quality参数只在图片类型是jpeg时生效<br/>
		/// </summary>
		/// <param name="image">Image object</param>
		/// <param name="filename">File path, will automatic create parent directories</param>
		/// <param name="quality">Compress quality, 1~100</param>
		/// <example>
		/// <code language="cs">
		/// image.SaveAuto("d:\\1.jpg", 90);
		/// </code>
		/// </example>
		public static void SaveAuto(this Image image, string filename, long quality) {
			PathUtils.EnsureParentDirectory(filename);
			var extension = Path.GetExtension(filename).ToLower();
			using (var stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write)) {
				image.SaveAuto(stream, extension, quality);
			}
		}

		/// <summary>
		/// Save image by it's file extension<br/>
		/// Quality parameter only available for jpeg<br/>
		/// 根据文件后缀保存图片<br/>
		/// quality参数只在图片类型是jpeg时生效<br/>
		/// </summary>
		/// <param name="image">Image object</param>
		/// <param name="stream">Stream object</param>
		/// <param name="extension">File extension, eg: ".jpg"</param>
		/// <param name="quality">Compress quality, 1~100</param>
		/// <example>
		/// <code language="cs">
		/// using (var stream = new MemoryStream()) {
		///		image.SaveIcon(stream);
		/// }
		/// </code>
		/// </example>
		public static void SaveAuto(this Image image, Stream stream, string extension, long quality) {
			if (extension == ".jpg" || extension == ".jpeg") {
				image.SaveJpeg(stream, quality);
			} else if (extension == ".bmp") {
				image.Save(stream, ImageFormat.Bmp);
			} else if (extension == ".gif") {
				image.Save(stream, ImageFormat.Gif);
			} else if (extension == ".ico") {
				image.SaveIcon(stream);
			} else if (extension == ".png") {
				image.Save(stream, ImageFormat.Png);
			} else if (extension == ".tiff") {
				image.Save(stream, ImageFormat.Tiff);
			} else if (extension == ".exif") {
				image.Save(stream, ImageFormat.Exif);
			} else {
				throw new NotSupportedException(
					string.Format("unsupport image extension {0}", extension));
			}
		}
	}

	/// <summary>
	/// Image resize mode<br/>
	/// 图片改变大小的模式<br/>
	/// </summary>
	public enum ImageResizeMode {
		/// <summary>
		/// Resize to the specified size, allow aspect ratio change<br/>
		/// 改变到指定大小, 允许纵横比的变更<br/>
		/// </summary>
		Fixed,
		/// <summary>
		/// Resize to the specified width, height is calculated by the aspect ratio<br/>
		/// 改变到指定宽度, 高度根据纵横比自动计算<br/>
		/// </summary>
		ByWidth,
		/// <summary>
		/// Resize to the specified height, width is calculated by the aspect ratio<br/>
		/// 改变到指定高度, 宽度根据纵横比自动计算<br/>
		/// </summary>
		ByHeight,
		/// <summary>
		/// Resize to the specified size, keep aspect ratio and cut the overflow part<br/>
		/// 改变到指定大小, 保持纵横比并剪切移除的部分<br/>
		/// </summary>
		Cut,
		/// <summary>
		/// Resize to the specified size, keep aspect ratio and padding the insufficient part<br/>
		/// 改变到指定大小, 保持纵横比并填充不足的部分<br/>
		/// </summary>
		Padding
	}
}
