using System;
using System.IO;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Stream extension methods<br/>
	/// 数据流的扩展函数<br/>
	/// </summary>
	public static class StreamExtensions {
		/// <summary>
		/// Copy stream to other stream from `begin` with `size` bytes<br/>
		/// 复制数据流到其他数据流, 从`begin`开始复制`size`个字节<br/>
		/// </summary>
		/// <param name="source">Source stream</param>
		/// <param name="destination">Destination stream</param>
		/// <param name="bufferSize">Buffer size</param>
		/// <param name="begin">Begin position</param>
		/// <param name="size">Copy size</param>
		/// <returns>The actual copied range</returns>
		/// <example>
		/// <code language="cs">
		/// using (var source = new MemoryStream(Encoding.UTF8.GetBytes("0123456789")))
		///	using (var desitination = new MemoryStream()) {
		///		source.CopyTo(desitination, 1024, 1, 3);
		///		desitination.Seek(0, SeekOrigin.Begin);
		///		var str = new StreamReader(desitination).ReadToEnd(); // "123"
		///
		///		desitination.Seek(0, SeekOrigin.Begin);
		///		source.CopyTo(desitination, 1024, 7, 5);
		///		desitination.Seek(0, SeekOrigin.Begin);
		///		str = new StreamReader(desitination).ReadToEnd(); // "789"
		///	}
		///	</code>
		/// </example>
		public static void CopyTo(
					this Stream source, Stream destination, int bufferSize, long begin, long size) {
			source.Seek(begin, SeekOrigin.Begin);
			var buffer = new byte[bufferSize];
			var sizeLeave = size;
			while (sizeLeave > 0) {
				int readed = source.Read(buffer, 0, (int)Math.Min(sizeLeave, bufferSize));
				if (readed <= 0) {
					return;
				}
				destination.Write(buffer, 0, readed);
				sizeLeave -= readed;
			}
		}
	}
}
