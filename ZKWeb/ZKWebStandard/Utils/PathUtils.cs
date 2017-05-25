using System;
using System.IO;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// Path utility functions<br/>
	/// 路径的工具函数<br/>
	/// </summary>
	public static class PathUtils {
		/// <summary>
		/// Secure path combining<br/>
		/// Throw exception if any part contains ".." or other invalid value<br/>
		/// 安全的合并路径<br/>
		/// 如果路径中有..或其他不合法的值则抛出例外<br/>
		/// </summary>
		/// <param name="paths">Path parts</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// PathUtils.SecureCombine("a", "b", "c") == Path.Combine("a", "b", "c")
		/// PathUtils.SecureCombine("a", "/b", "c") throws exception
		/// PathUtils.SecureCombine("a", "\\b", "c") throws exception
		/// PathUtils.SecureCombine("a", "", "c") throws exception
		/// PathUtils.SecureCombine("a", "..", "c") throws exception
		/// PathUtils.SecureCombine("a/../b", "c") throws exception
		/// </code>
		/// </example>
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
		/// 确保路径的上级文件夹存在<br/>
		/// </summary>
		/// <param name="path">Path</param>
		/// <example>
		/// <code language="cs">
		/// PathUtils.EnsureParentDirectory("c:\abc\123.txt");
		/// // will create c:\abc if not exist
		/// </code>
		/// </example>
		public static void EnsureParentDirectory(string path) {
			var parentDirectory = Path.GetDirectoryName(path);
			if (!Directory.Exists(parentDirectory)) {
				Directory.CreateDirectory(parentDirectory);
			}
		}
	}
}
