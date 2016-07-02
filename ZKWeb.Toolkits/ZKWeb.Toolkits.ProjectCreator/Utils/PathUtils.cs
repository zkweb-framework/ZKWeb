using System.IO;

namespace ZKWeb.Toolkits.ProjectCreator.Utils {
	/// <summary>
	/// 路径的工具函数
	/// </summary>
	public static class PathUtils {
		/// <summary>
		/// 生成相对路径
		/// </summary>
		/// <param name="fromPath">来源路径</param>
		/// <param name="toPath">目标路径</param>
		/// <returns></returns>
		public static string MakeRelativePath(string fromPath, string toPath) {
			fromPath = Path.GetFullPath(fromPath);
			toPath = Path.GetFullPath(toPath);
			// 一层层提升来源路径，直到目标路径以来源路径为前缀
			var resultPath = "";
			while (!toPath.StartsWith(fromPath)) {
				fromPath = Path.GetDirectoryName(fromPath);
				resultPath += "../";
				if (string.IsNullOrEmpty(fromPath)) {
					// 无法找到共同的前缀，可能不在同一个驱动器上
					return toPath;
				}
			}
			// 添加目标路径
			var subPart = toPath.Substring(fromPath.Length);
			subPart = subPart.Replace(Path.DirectorySeparatorChar, '/');
			if (subPart.StartsWith("/")) {
				subPart = subPart.Substring(1);
			}
			resultPath += subPart;
			return resultPath;
		}
	}
}
