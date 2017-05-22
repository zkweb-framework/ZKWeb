using System;
using System.IO;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Path utility functions<br/>
	/// <br/>
	/// </summary>
	public static class PathUtils {
		/// <summary>
		/// Secure path combining<br/>
		/// Throw exception if any part contains ".." or other invalid value<br/>
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="paths">Path parts</param>
		/// <returns></returns>
		public static string SecureCombine(params string[] paths) {
			for (var i = 0; i < paths.Length; ++i) {
				var path = paths[i];
				if (i > 0 && path.StartsWith("/")) {
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
		/// Ensure parent directories are exist<br/>
		/// <br/>
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
