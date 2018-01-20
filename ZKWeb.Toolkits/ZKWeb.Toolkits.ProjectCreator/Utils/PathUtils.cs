using System.IO;
using System.Text;

namespace ZKWeb.Toolkits.ProjectCreator.Utils {
	/// <summary>
	/// Path utility functions
	/// </summary>
	public static class PathUtils {
		/// <summary>
		/// Make relative path
		/// </summary>
		/// <param name="fromPath">From path</param>
		/// <param name="toPath">To path</param>
		/// <returns></returns>
		public static string MakeRelativePath(string fromPath, string toPath) {
			fromPath = Path.GetFullPath(fromPath);
			toPath = Path.GetFullPath(toPath);
			// Get parent paths of fromPath, until toPath starts with fromPath
			var resultPath = new StringBuilder();
			while (!toPath.StartsWith(fromPath)) {
				fromPath = Path.GetDirectoryName(fromPath);
				resultPath.Append("../");
				if (string.IsNullOrEmpty(fromPath)) {
					// No common prefix, maybe not on same driver
					return toPath;
				}
			}
			// Add non common part from toPath
			var subPart = toPath.Substring(fromPath.Length);
			subPart = subPart.Replace(Path.DirectorySeparatorChar, '/');
			if (subPart.StartsWith("/")) {
				subPart = subPart.Substring(1);
			}
			resultPath.Append(subPart);
			return resultPath.ToString();
		}
	}
}
