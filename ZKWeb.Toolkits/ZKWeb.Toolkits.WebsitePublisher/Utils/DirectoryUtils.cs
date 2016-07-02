using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ZKWeb.Toolkits.WebsitePublisher.Utils {
	/// <summary>
	/// 文件夹的工具函数
	/// </summary>
	public static class DirectoryUtils {
		/// <summary>
		/// 递归复制文件夹内容
		/// </summary>
		/// <param name="fromDir">来源文件夹</param>
		/// <param name="toDir">目标文件夹</param>
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
