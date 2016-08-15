using System;
using System.IO;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Path utility functions
	/// </summary>
	public static class PathUtils {
		/// <summary>
		/// Secure path combining
		/// Throw exception if any part contains ".." or other invalid value
		/// </summary>
		/// <param name="paths">Path parts</param>
		/// <returns></returns>
		public static string SecureCombine(params string[] paths) {
			foreach (var path in paths) {
				if (path.StartsWith("/")) {
					throw new ArgumentException($"path startswith '/'");
				} else if (path.StartsWith("\\")) {
					throw new ArgumentException($"path startswith '\'");
				} else if (string.IsNullOrEmpty(path)) {
					throw new ArgumentException($"path {path} is null or empty");
				} else if (path.Contains("..")) {
					throw new ArgumentException($"path {path} contains '..'");
				}
			}
			return Path.Combine(paths);
		}

		/// <summary>
		/// Ensure parent directories are exist
		/// </summary>
		/// <param name="path">Path</param>
		public static void EnsureParentDirectory(string path) {
			var parentDirectory = Path.GetDirectoryName(path);
			if (!Directory.Exists(parentDirectory)) {
				Directory.CreateDirectory(parentDirectory);
			}
		}
	}
}
