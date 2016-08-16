using System.IO;

namespace ZKWeb.Toolkits.WebsitePublisher.Utils {
	/// <summary>
	/// Directory utility functions
	/// </summary>
	public static class DirectoryUtils {
		/// <summary>
		/// Copy directory recursivly
		/// </summary>
		/// <param name="fromDir">From directory</param>
		/// <param name="toDir">To directory</param>
		public static void CopyDirectory(string fromDir, string toDir) {
			foreach (var path in Directory.EnumerateFiles(fromDir, "*", SearchOption.AllDirectories)) {
				var relPath = path.Substring(fromDir.Length + 1);
				var toPath = Path.Combine(toDir, relPath);
				Directory.CreateDirectory(Path.GetDirectoryName(toPath));
				File.Copy(path, toPath, true);
			}
		}
	}
}
