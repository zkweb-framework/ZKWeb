using System;
using System.IO;
using System.Reflection;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// 路径工具类
	/// </summary>
	public static class PathUtils {
		/// <summary>
		/// 安全的组合路径列表
		/// 检查参数是否为空或包含..
		/// </summary>
		/// <param name="paths">路径列表</param>
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
		/// 检查路径的上一级路径是否存在，不存在时创建
		/// </summary>
		/// <param name="path">路径</param>
		public static void EnsureParentDirectory(string path) {
			var parentDirectory = Path.GetDirectoryName(path);
			if (!Directory.Exists(parentDirectory)) {
				Directory.CreateDirectory(parentDirectory);
			}
		}
	}
}
