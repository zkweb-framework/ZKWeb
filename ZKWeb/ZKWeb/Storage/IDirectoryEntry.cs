using System.Collections.Generic;

namespace ZKWeb.Storage {
	/// <summary>
	/// Interface of directory entry<br/>
	/// 目录的接口<br/>
	/// </summary>
	/// <seealso cref="IFileStorage"/>
	/// <seealso cref="IFileEntry"/>
	public interface IDirectoryEntry {
		/// <summary>
		/// Directory name<br/>
		/// 目录名称<br/>
		/// </summary>
		string DirectoryName { get; }
		/// <summary>
		/// Check if directory is exist<br/>
		/// 检查目录是否存在<br/>
		/// </summary>
		bool Exists { get; }
		/// <summary>
		/// Enumerate files<br/>
		/// 枚举目录下的文件<br/>
		/// </summary>
		/// <returns></returns>
		IEnumerable<IFileEntry> EnumerateFiles();
		/// <summary>
		/// Enumerate directories<br/>
		/// 枚举目录下的目录<br/>
		/// </summary>
		/// <returns></returns>
		IEnumerable<IDirectoryEntry> EnumerateDirectories();
		/// <summary>
		/// Delete this directory and all files under it<br/>
		/// 删除目录和目录下的所有文件<br/>
		/// </summary>
		void Delete();
	}
}
