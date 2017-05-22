using System;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// File utility functions<br/>
	/// <br/>
	/// </summary>
	public static class FileUtils {
		/// <summary>
		/// Get display name for bytes<br/>
		/// <br/>
		/// Eg: 123KB, 123MB, 123GB
		/// </summary>
		/// <param name="bytes">bytes</param>
		/// <returns></returns>
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
