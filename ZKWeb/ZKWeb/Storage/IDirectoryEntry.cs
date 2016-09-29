using System.Collections.Generic;

namespace ZKWeb.Storage {
	/// <summary>
	/// Interface for directory entry
	/// </summary>
	public interface IDirectoryEntry {
		/// <summary>
		/// Directory name
		/// </summary>
		string DirectoryName { get; }
		/// <summary>
		/// Check if directory is exist
		/// </summary>
		bool Exists { get; }
		/// <summary>
		/// Enumerate files
		/// </summary>
		/// <returns></returns>
		IEnumerable<IFileEntry> EnumerateFiles();
		/// <summary>
		/// Enumerate directories
		/// </summary>
		/// <returns></returns>
		IEnumerable<IDirectoryEntry> EnumerateDirectories();
		/// <summary>
		/// Delete this directory and all files under it
		/// </summary>
		void Delete();
	}
}
