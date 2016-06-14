using System;

namespace ZKWebStandard.Utils {
	/// <summary>
	/// 文件工具类
	/// </summary>
	public static class FileUtils {
		/// <summary>
		/// 获取字节大小的显示名称
		/// 自动转换成KB、MB或GB格式
		/// </summary>
		/// <param name="bytes">字节数</param>
		/// <returns></returns>
		public static string GetSizeDisplayName(long bytes) {
			if (bytes >= 1073741824) {
				return $"{Decimal.Round((decimal)bytes / 1073741824, 2)}GB";
			} else if (bytes >= 1048576) {
				return $"{Decimal.Round((decimal)bytes / 1048576, 2)}MB";
			} else if (bytes >= 1024) {
				return $"{Decimal.Round((decimal)bytes / 1024, 2)}KB";
			}
			return $"{bytes}Bytes";
		}
	}
}
