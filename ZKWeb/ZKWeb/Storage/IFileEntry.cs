using System;
using System.IO;

namespace ZKWeb.Storage {
	/// <summary>
	/// Interface for file entry
	/// </summary>
	public interface IFileEntry {
		/// <summary>
		/// Filename
		/// </summary>
		string Filename { get; }
		/// <summary>
		/// Unique identifier, dependents on file storage
		/// </summary>
		string UniqueIdentifier { get; }
		/// <summary>
		/// Check if file is exist
		/// </summary>
		bool Exist { get; }
		/// <summary>
		/// Get file creation time in UTC
		/// </summary>
		DateTime CreationTimeUtc { get; }
		/// <summary>
		/// Get file last access time in UTC
		/// </summary>
		DateTime LastAccessTimeUtc { get; }
		/// <summary>
		/// Get file last write time in UTC
		/// </summary>
		DateTime LastWriteTimeUtc { get; }
		/// <summary>
		/// Open file for read
		/// May throw exception if unsupported
		/// </summary>
		/// <returns></returns>
		Stream OpenRead();
		/// <summary>
		/// Open file for write
		/// May throw exception if unsupported
		/// </summary>
		/// <returns></returns>
		Stream OpenWrite();
		/// <summary>
		/// Open file for append
		/// May throw exception if unsupported
		/// </summary>
		/// <returns></returns>
		Stream OpenAppend();
		/// <summary>
		/// Delete this file
		/// May throw exception if unsupported
		/// </summary>
		void Delete();
	}
}
