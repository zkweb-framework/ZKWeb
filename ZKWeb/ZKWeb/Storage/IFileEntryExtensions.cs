using System.IO;

namespace ZKWeb.Storage {
	/// <summary>
	/// File entry extension methods
	/// </summary>
	public static class IFileEntryExtensions {
		/// <summary>
		/// Read file into string
		/// </summary>
		/// <param name="entry">File entry</param>
		/// <returns></returns>
		public static string ReadAllText(this IFileEntry entry) {
			using (var stream = entry.OpenRead())
			using (var reader = new StreamReader(stream)) {
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Write string to file
		/// </summary>
		/// <param name="entry">File entry</param>
		/// <param name="value">String value</param>
		public static void WriteAllText(this IFileEntry entry, string value) {
			using (var stream = entry.OpenWrite())
			using (var writer = new StreamWriter(stream)) {
				writer.Write(value);
			}
		}

		/// <summary>
		/// Append string to file
		/// </summary>
		/// <param name="entry">File entry</param>
		/// <param name="value">String value</param>
		public static void AppendAllText(this IFileEntry entry, string value) {
			using (var stream = entry.OpenAppend())
			using (var writer = new StreamWriter(stream)) {
				writer.Write(value);
			}
		}

		/// <summary>
		/// Read file into byte array
		/// </summary>
		/// <param name="entry">File entry</param>
		/// <returns></returns>
		public static byte[] ReadAllBytes(this IFileEntry entry) {
			using (var stream = entry.OpenRead())
			using (var memoryStream = new MemoryStream()) {
				stream.CopyTo(memoryStream);
				return memoryStream.ToArray();
			}
		}

		/// <summary>
		/// Write byte array to file
		/// </summary>
		/// <param name="entry">File entry</param>
		/// <param name="bytes">Byte array</param>
		public static void WriteAllBytes(this IFileEntry entry, byte[] bytes) {
			using (var stream = entry.OpenRead()) {
				stream.Write(bytes, 0, bytes.Length);
			}
		}
	}
}
