namespace ZKWeb.Storage {
	/// <summary>
	/// Local file storage
	/// </summary>
	internal class LocalFileStorage : IFileStorage {
		/// <summary>
		/// Open template file, it should be readonly
		/// </summary>
		public IFileEntry GetTemplateFile(string path) {
			var pathManager = Application.Ioc.Resolve<LocalPathManager>();
			var fullPath = pathManager.GetTemplateFullPath(path);
			return new LocalFileEntry(fullPath, true);
		}

		/// <summary>
		/// Open resource file, it should be readonly
		/// </summary>
		public IFileEntry GetResourceFile(params string[] pathParts) {
			var pathManager = Application.Ioc.Resolve<LocalPathManager>();
			var fullPath = pathManager.GetResourceFullPath(pathParts);
			return new LocalFileEntry(fullPath, true);
		}

		/// <summary>
		/// Open storage file
		/// </summary>
		public IFileEntry GetStorageFile(params string[] pathParts) {
			var pathManager = Application.Ioc.Resolve<LocalPathManager>();
			var fullPath = pathManager.GetStorageFullPath(pathParts);
			return new LocalFileEntry(fullPath, false);
		}

		/// <summary>
		/// Enumerate directories
		/// </summary>
		public IDirectoryEntry GetStorageDirectory(params string[] pathParts) {
			var pathManager = Application.Ioc.Resolve<LocalPathManager>();
			var fullPath = pathManager.GetStorageFullPath(pathParts);
			return new LocalDirectoryEntry(fullPath);
		}
	}
}
