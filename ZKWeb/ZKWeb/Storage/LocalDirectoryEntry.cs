using System.Collections.Generic;
using System.IO;

namespace ZKWeb.Storage {
	/// <summary>
	/// Local directory entry<br/>
	/// 本地文件系统上的目录<br/>
	/// </summary>
	/// <seealso cref="IFileStorage"/>
	/// <seealso cref="IFileEntry"/>
	public class LocalDirectoryEntry : IDirectoryEntry {
		/// <summary>
		/// Full path<br/>
		/// 完整路径<br/>
		/// </summary>
		private string FullPath { get; set; }
		/// <summary>
		/// Directory name<br/>
		/// 目录名称<br/>
		/// </summary>
		public string DirectoryName { get { return Path.GetFileName(FullPath); } }
		/// <summary>
		/// Check if directory is exist<br/>
		/// 检查目录是否存在<br/>
		/// </summary>
		public bool Exists { get { return Directory.Exists(FullPath); } }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="fullPath">Full path</param>
		public LocalDirectoryEntry(string fullPath) {
			FullPath = fullPath;
		}

		/// <summary>
		/// Enumerate files<br/>
		/// 枚举目录下的文件<br/>
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IDirectoryEntry> EnumerateDirectories() {
			if (!Exists) {
				yield break;
			}
			foreach (var path in Directory.EnumerateDirectories(FullPath)) {
				yield return new LocalDirectoryEntry(path);
			}
		}

		/// <summary>
		/// Enumerate directories<br/>
		/// 枚举目录下的目录<br/>
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IFileEntry> EnumerateFiles() {
			if (!Exists) {
				yield break;
			}
			foreach (var path in Directory.EnumerateFiles(FullPath)) {
				yield return new LocalFileEntry(path, false);
			}
		}

		/// <summary>
		/// Delete this directory and all files under it<br/>
		/// 删除目录和目录下的所有文件<br/>
		/// </summary>
		public void Delete() {
			if (Exists) {
				Directory.Delete(FullPath, true);
			}
		}
	}
}
