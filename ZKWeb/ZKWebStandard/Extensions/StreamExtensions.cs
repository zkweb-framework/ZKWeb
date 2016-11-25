using System;
using System.IO;

namespace ZKWebStandard.Extensions {
	/// <summary>
	/// Stream extension methods
	/// </summary>
	public static class StreamExtensions {
		/// <summary>
		/// Copy stream to other stream from `begin` with `size` bytes
		/// </summary>
		/// <param name="source">Source stream</param>
		/// <param name="destination">Destination stream</param>
		/// <param name="bufferSize">Buffer size</param>
		/// <param name="begin">Begin position</param>
		/// <param name="size">Copy size</param>
		/// <returns>The actual copied range</returns>
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
