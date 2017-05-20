using System;
using System.IO;

namespace ZKWeb.Storage {
	/// <summary>
	/// Interface of file entry<br/>
	/// 文件的接口<br/>
	/// </summary>
	/// <seealso cref="IFileStorage"/>
	/// <seealso cref="IDirectoryEntry"/>
	public interface IFileEntry {
		/// <summary>
		/// Filename, not included full path<br/>
		/// 文件名, 不包含完整路径<br/>
		/// </summary>
		string Filename { get; }
		/// <summary>
		/// Unique identifier, dependents on file storage<br/>
		/// 唯一标识符, 根据文件系统而定<br/>
		/// </summary>
		string UniqueIdentifier { get; }
		/// <summary>
		/// Check if file is exist<br/>
		/// 检查文件是否存在<br/>
		/// </summary>
		bool Exists { get; }
		/// <summary>
		/// Get file creation time in UTC, not available on *unix<br/>
		/// 获取文件的创建时间, *unix系统上不可用<br/>
		/// </summary>
		DateTime CreationTimeUtc { get; }
		/// <summary>
		/// Get file last access time in UTC<br/>
		/// 获取文件的最后访问时间<br/>
		/// </summary>
		DateTime LastAccessTimeUtc { get; }
		/// <summary>
		/// Get file last write time in UTC<br/>
		/// 获取文件的最后写入时间<br/>
		/// </summary>
		DateTime LastWriteTimeUtc { get; }
		/// <summary>
		/// Get file length in bytes<br/>
		/// 获取文件长度, 单位是字节<br/>
		/// </summary>
		long Length { get; }
		/// <summary>
		/// Open file for read<br/>
		/// Exception is thrown if the action is not supported<br/>
		/// 打开文件用于读取内容<br/>
		/// 如果不支持则抛出例外<br/>
		/// </summary>
		/// <returns></returns>
		Stream OpenRead();
		/// <summary>
		/// Open file for write<br/>
		/// Exception is thrown if the action is not supported<br/>
		/// 打开文件用于写入内容<br/>
		/// 如果不支持则抛出例外<br/>
		/// </summary>
		/// <returns></returns>
		Stream OpenWrite();
		/// <summary>
		/// Open file for append<br/>
		/// Exception is thrown if the action is not supported<br/>
		/// 打开文件用于追加内容<br/>
		/// 如果不支持则抛出例外<br/>
		/// </summary>
		/// <returns></returns>
		Stream OpenAppend();
		/// <summary>
		/// Delete this file<br/>
		/// Exception is thrown if the action is not supported<br/>
		/// 删除文件<br/>
		/// 如果不支持则抛出例外<br/>
		/// </summary>
		void Delete();
	}
}
