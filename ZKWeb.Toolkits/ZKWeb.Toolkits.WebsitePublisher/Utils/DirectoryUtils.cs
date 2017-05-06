using System;
using System.IO;
using System.Text.RegularExpressions;

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
		/// <param name="ignorePattern">Ignore pattern in regex</param>
		public static void CopyDirectory(string fromDir, string toDir, Regex ignorePattern) {
			foreach (var path in Directory.EnumerateFileSystemEntries(
				fromDir, "*", SearchOption.TopDirectoryOnly)) {
				if (ignorePattern != null && ignorePattern.IsMatch(path)) {
					continue;
				}
				var relPath = path.Substring(fromDir.Length + 1);
				var toPath = Path.Combine(toDir, relPath);
				try {
					if (File.Exists(path)) {
						Directory.CreateDirectory(Path.GetDirectoryName(toPath));
						File.Copy(path, toPath, true);
					} else if (Directory.Exists(path)) {
						CopyDirectory(path, toPath, ignorePattern);
					}
				} catch (PathTooLongException e) {
					Console.WriteLine(e.ToString());
				}
			}
		}
	}
}
