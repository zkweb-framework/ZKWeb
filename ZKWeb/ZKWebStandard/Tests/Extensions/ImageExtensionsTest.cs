using System;
using System.DrawingCore;
using System.IO;
using ZKWebStandard.Extensions;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Extensions {
	[Tests]
	class ImageExtensionsTest {
		public void Resize() {
			using (var image = new Bitmap(3, 2)) {
				// blue, black, red
				// red, blue, black
				image.SetPixel(0, 0, Color.Blue);
				image.SetPixel(1, 0, Color.Black);
				image.SetPixel(2, 0, Color.Red);
				image.SetPixel(0, 1, Color.Red);
				image.SetPixel(1, 1, Color.Blue);
				image.SetPixel(2, 1, Color.Black);
				// Fixed
				using (var newImage = (Bitmap)image.Resize(3, 3, ImageResizeMode.Fixed)) {
					Assert.Equals(newImage.Size, new Size(3, 3));
					Assert.IsTrue(newImage.GetPixel(0, 0).A != 0);
					Assert.IsTrue(newImage.GetPixel(2, 2).A != 0);
				}
				// ByWidth
				using (var newImage = (Bitmap)image.Resize(9, 100, ImageResizeMode.ByWidth)) {
					Assert.Equals(newImage.Size, new Size(9, 6));
					Assert.IsTrue(newImage.GetPixel(0, 0).A != 0);
					Assert.IsTrue(newImage.GetPixel(8, 5).A != 0);
				}
				// ByHeight
				using (var newImage = (Bitmap)image.Resize(100, 6, ImageResizeMode.ByHeight)) {
					Assert.Equals(newImage.Size, new Size(9, 6));
					Assert.IsTrue(newImage.GetPixel(0, 0).A != 0);
					Assert.IsTrue(newImage.GetPixel(8, 5).A != 0);
				}
				// Cut
				using (var newImage = (Bitmap)image.Resize(1, 2, ImageResizeMode.Cut)) {
					Assert.Equals(newImage.Size, new Size(1, 2));
					Assert.Equals(newImage.GetPixel(0, 0).ToArgb(), Color.Black.ToArgb());
					Assert.Equals(newImage.GetPixel(0, 1).ToArgb(), Color.Blue.ToArgb());
				}
				// Padding
				using (var newImage = (Bitmap)image.Resize(5, 2, ImageResizeMode.Padding)) {
					Assert.Equals(newImage.Size, new Size(5, 2));
					Assert.Equals(newImage.GetPixel(0, 0).A, 0);
					Assert.Equals(newImage.GetPixel(0, 1).A, 0);
					Assert.Equals(newImage.GetPixel(1, 0).ToArgb(), Color.Blue.ToArgb());
					Assert.Equals(newImage.GetPixel(1, 1).ToArgb(), Color.Red.ToArgb());
					Assert.Equals(newImage.GetPixel(3, 1).ToArgb(), Color.Black.ToArgb());
					Assert.Equals(newImage.GetPixel(4, 0).A, 0);
					Assert.Equals(newImage.GetPixel(4, 0).A, 0);
				}
			}
		}

		public void SaveJpeg() {
			using (var image = new Bitmap(3, 2)) {
				var path = Path.GetTempFileName();
#pragma warning disable CS0618
				image.SaveJpeg(path, 100);
#pragma warning restore CS0618
				var fileInfo = new FileInfo(path);
				Assert.IsTrueWith(fileInfo.Length > 0, fileInfo);
				File.Delete(path);
			}
		}

		public void SaveIcon() {
			using (var image = new Bitmap(3, 2)) {
				var path = Path.GetTempFileName();
#pragma warning disable CS0618
				image.SaveIcon(path);
#pragma warning restore CS0618
				var fileInfo = new FileInfo(path);
				Assert.IsTrueWith(fileInfo.Length > 0, fileInfo);
				File.Delete(path);
			}
		}

		public void SaveAuto() {
			using (var image = new Bitmap(3, 2)) {
				var path = Path.GetTempFileName() + ".bmp";
				image.SaveAuto(path, 100);
				var fileInfo = new FileInfo(path);
				Assert.IsTrueWith(fileInfo.Length > 0, fileInfo);
				File.Delete(path);

				path = Path.GetTempFileName() + ".unknown";
				Assert.Throws<NotSupportedException>(() => image.SaveAuto(path, 100));
				File.Delete(path);
			}
		}
	}
}
