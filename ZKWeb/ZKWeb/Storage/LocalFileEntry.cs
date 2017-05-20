using System;
using System.IO;
using ZKWebStandard.Utils;

namespace ZKWeb.Storage {
	/// <summary>
	/// Local file entry<br/>
	/// 本地文件系统上的文件<br/>
	/// </summary>
	/// <seealso cref="IFileStorage"/>
	/// <seealso cref="IDirectoryEntry"/>
	public class LocalFileEntry : IFileEntry {
		/// <summary>
		/// Full path<br/>
		/// 完整路径<br/>
		/// </summary>
		private string FullPath { get; set; }
		/// <summary>
		/// Is file readonly<br/>
		/// 文件是否只读<br/>
		/// </summary>
		private bool ReadOnly { get; set; }
		/// <summary>
		/// Filename, not included full path<br/>
		/// 文件名, 不包含完整路径<br/>
		/// </summary>
		public string Filename { get { return Path.GetFileName(FullPath); } }
		/// <summary>
		/// Unique identifier, dependents on file storage<br/>
		/// 唯一标识符, 根据文件系统而定<br/>
		/// </summary>
		public string UniqueIdentifier { get { return FullPath; } }
		/// <summary>
		/// Check if file is exist<br/>
		/// 检查文件是否存在<br/>
		/// </summary>
		public bool Exists { get { return File.Exists(FullPath); } }
		/// <summary>
		/// Get file creation time in UTC, not available on *unix<br/>
		/// 获取文件的创建时间, *unix系统上不可用<br/>
		/// </summary>
		public DateTime CreationTimeUtc { get { return File.GetCreationTimeUtc(FullPath); } }
		/// <summary>
		/// Get file last access time in UTC<br/>
		/// 获取文件的最后访问时间<br/>
		/// </summary>
		public DateTime LastAccessTimeUtc { get { return File.GetLastAccessTimeUtc(FullPath); } }
		/// <summary>
		/// Get file last write time in UTC<br/>
		/// 获取文件的最后写入时间<br/>
		/// </summary>
		public DateTime LastWriteTimeUtc { get { return File.GetLastWriteTimeUtc(FullPath); } }
		/// <summary>
		/// Get file length in bytes<br/>
		/// 获取文件长度, 单位是字节<br/>
		/// </summary>
		public long Length { get { return new FileInfo(FullPath).Length; } }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="fullPath">Full path</param>
		/// <param name="readOnly">Is file readonly</param>
		public LocalFileEntry(string fullPath, bool readOnly) {
			FullPath = fullPath;
			ReadOnly = readOnly;
		}

		/// <summary>
		/// Open file for read<br/>
		/// Exception is thrown if the action is not supported<br/>
		/// 打开文件用于读取内容<br/>
		/// 如果不支持则抛出例外<br/>
		/// </summary>
		/// <returns></returns>
		public Stream OpenRead() {
			return new FileStream(FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		/// <summary>
		/// Open file for write<br/>
		/// Exception is thrown if the action is not supported<br/>
		/// 打开文件用于写入内容<br/>
		/// 如果不支持则抛出例外<br/>
		/// </summary>
		/// <returns></returns>
		public Stream OpenWrite() {
			if (ReadOnly) {
				throw new NotSupportedException("This file is readonly");
			}
			PathUtils.EnsureParentDirectory(FullPath);
			return new FileStream(FullPath, FileMode.Create, FileAccess.Write, FileShare.None);
		}

		/// <summary>
		/// Open file for append<br/>
		/// Exception is thrown if the action is not supported<br/>
		/// 打开文件用于追加内容<br/>
		/// 如果不支持则抛出例外<br/>
		/// </summary>
		/// <returns></returns>
		public Stream OpenAppend() {
			if (ReadOnly) {
				throw new NotSupportedException("This file is readonly");
			}
			PathUtils.EnsureParentDirectory(FullPath);
			return new FileStream(FullPath, FileMode.Append, FileAccess.Write, FileShare.None);
		}

		/// <summary>
		/// Delete this file<br/>
		/// Exception is thrown if the action is not supported<br/>
		/// 删除文件<br/>
		/// 如果不支持则抛出例外<br/>
		/// </summary>
		public void Delete() {
			if (ReadOnly) {
				throw new NotSupportedException("This file is readonly");
			} else if (Exists) {
				File.Delete(FullPath);
			}
		}
	}
}
