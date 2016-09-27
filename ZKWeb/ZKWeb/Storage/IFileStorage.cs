namespace ZKWeb.Storage {
	/// <summary>
	/// Interface for file storage
	/// </summary>
	public interface IFileStorage {
		/// <summary>
		/// Get template file, it should be readonly
		/// </summary>
		/// <param name="path">Template file path</param>
		/// <returns></returns>
		IFileEntry GetTemplateFile(string path);

		/// <summary>
		/// Get resource file, it should be readonly
		/// </summary>
		/// <param name="pathParts">Resource file path parts</param>
		/// <returns></returns>
		IFileEntry GetResourceFile(params string[] pathParts);

		/// <summary>
		/// Get storage file
		/// </summary>
		/// <param name="pathParts">Storage file path parts</param>
		/// <returns></returns>
		IFileEntry GetStorageFile(params string[] pathParts);

		/// <summary>
		/// Get storage directory
		/// </summary>
		/// <param name="pathParts">Storage directory path parts</param>
		IDirectoryEntry GetStorageDirectory(params string[] pathParts);
	}
}
