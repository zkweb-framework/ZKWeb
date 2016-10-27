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
		public static void CopyDirectory(string fromDir, string toDir, string ignorePattern) {
			var regex = new Regex(ignorePattern);
			foreach (var path in Directory.EnumerateFiles(fromDir, "*", SearchOption.AllDirectories)) {
				if (!string.IsNullOrEmpty(ignorePattern) && regex.IsMatch(path)) {
					continue;
				}
				var relPath = path.Substring(fromDir.Length + 1);
				var toPath = Path.Combine(toDir, relPath);
				try {
					Directory.CreateDirectory(Path.GetDirectoryName(toPath));
					File.Copy(path, toPath, true);
				} catch (PathTooLongException e) {
					Console.WriteLine(e.ToString());
				}
			}
		}
	}
}
