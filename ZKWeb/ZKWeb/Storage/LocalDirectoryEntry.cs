using System.Collections.Generic;
using System.IO;

namespace ZKWeb.Storage {
	/// <summary>
	/// Local directory entry
	/// </summary>
	public class LocalDirectoryEntry : IDirectoryEntry {
		/// <summary>
		/// Full path
		/// </summary>
		private string FullPath { get; set; }
		/// <summary>
		/// Directory name
		/// </summary>
		public string DirectoryName { get { return Path.GetFileName(FullPath); } }
		/// <summary>
		/// Check if directory is exist
		/// </summary>
		public bool Exists { get { return Directory.Exists(FullPath); } }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="fullPath">Full path</param>
		public LocalDirectoryEntry(string fullPath) {
			FullPath = fullPath;
		}

		/// <summary>
		/// Enumerate directories
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
		/// Enumerate files
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
		/// Delete this directory and all files under it
		/// </summary>
		public void Delete() {
			if (Exists) {
				Directory.Delete(FullPath, true);
			}
		}
	}
}
