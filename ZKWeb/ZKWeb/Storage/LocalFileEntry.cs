using System;
using System.IO;
using ZKWebStandard.Utils;

namespace ZKWeb.Storage {
	/// <summary>
	/// Local file entry
	/// </summary>
	public class LocalFileEntry : IFileEntry {
		/// <summary>
		/// Full path
		/// </summary>
		private string FullPath { get; set; }
		/// <summary>
		/// Is file readonly
		/// </summary>
		private bool ReadOnly { get; set; }
		/// <summary>
		/// Filename
		/// </summary>
		public string Filename { get { return Path.GetFileName(FullPath); } }
		/// <summary>
		/// Unique identifier, dependents on file storage
		/// </summary>
		public string UniqueIdentifier { get { return FullPath; } }
		/// <summary>
		/// Check if file is exist
		/// </summary>
		public bool Exist { get { return File.Exists(FullPath); } }
		/// <summary>
		/// Get file creation time in UTC
		/// </summary>
		public DateTime CreationTimeUtc { get { return File.GetCreationTimeUtc(FullPath); } }
		/// <summary>
		/// Get file last access time in UTC
		/// </summary>
		public DateTime LastAccessTimeUtc { get { return File.GetLastAccessTimeUtc(FullPath); } }
		/// <summary>
		/// Get file last write time in UTC
		/// </summary>
		public DateTime LastWriteTimeUtc { get { return File.GetLastWriteTimeUtc(FullPath); } }

		/// <summary>
		/// Initialize
		/// </summary>
		/// <param name="fullPath">Full path</param>
		/// <param name="readOnly">Is file readonly</param>
		public LocalFileEntry(string fullPath, bool readOnly) {
			FullPath = fullPath;
			ReadOnly = readOnly;
		}

		/// <summary>
		/// Open file for read
		/// </summary>
		/// <returns></returns>
		public Stream OpenRead() {
			return new FileStream(FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		/// <summary>
		/// Open file for write
		/// </summary>
		/// <returns></returns>
		public Stream OpenWrite() {
			if (ReadOnly) {
				throw new NotSupportedException("This file is readonly");
			}
			PathUtils.EnsureParentDirectory(FullPath);
			return new FileStream(FullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
		}

		/// <summary>
		/// Open file for append
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
		/// Delete this file
		/// </summary>
		public void Delete() {
			if (ReadOnly) {
				throw new NotSupportedException("This file is readonly");
			}
			File.Delete(FullPath);
		}
	}
}
