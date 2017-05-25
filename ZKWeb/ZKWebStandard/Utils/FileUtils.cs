using System;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// File utility functions<br/>
	/// 文件的工具函数<br/>
	/// </summary>
	public static class FileUtils {
		/// <summary>
		/// Get display name for bytes<br/>
		/// 获取字节的表示名称<br/>
		/// Eg: 123KB, 123MB, 123GB
		/// </summary>
		/// <param name="bytes">bytes</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// FileUtils.GetSizeDisplayName(0) == "0Bytes"
		/// FileUtils.GetSizeDisplayName(1023) == "1023Bytes"
		/// FileUtils.GetSizeDisplayName(1024) == "1KB"
		/// FileUtils.GetSizeDisplayName((int)(1024 * 1.5M)) == "1.5KB"
		/// FileUtils.GetSizeDisplayName(1048576) == "1MB"
		/// FileUtils.GetSizeDisplayName((int)(1048576 * 1.5M)) == "1.5MB"
		/// FileUtils.GetSizeDisplayName(1073741824) == "1GB"
		/// FileUtils.GetSizeDisplayName((int)(1073741824 * 1.5M)) == "1.5GB"
		/// </code>
		/// </example>
		public static string GetSizeDisplayName(long bytes) {
			if (bytes >= 1073741824) {
				return $"{Math.Round((decimal)bytes / 1073741824, 2)}GB";
			} else if (bytes >= 1048576) {
				return $"{Math.Round((decimal)bytes / 1048576, 2)}MB";
			} else if (bytes >= 1024) {
				return $"{Math.Round((decimal)bytes / 1024, 2)}KB";
			}
			return $"{bytes}Bytes";
		}
	}
}
