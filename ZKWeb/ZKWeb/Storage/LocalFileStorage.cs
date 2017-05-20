namespace ZKWeb.Storage {
	/// <summary>
	/// Local file storage<br/>
	/// 本地的文件系统<br/>
	/// </summary>
	internal class LocalFileStorage : IFileStorage {
		/// <summary>
		/// Get template file, it should be readonly<br/>
		/// 获取模板文件, 返回的文件应该是只读的<br/>
		/// </summary>
		public IFileEntry GetTemplateFile(string path) {
			var pathManager = Application.Ioc.Resolve<LocalPathManager>();
			var fullPath = pathManager.GetTemplateFullPath(path);
			return new LocalFileEntry(fullPath, true);
		}

		/// <summary>
		/// Get resource file, it should be readonly<br/>
		/// 获取资源文件, 返回的文件应该是只读的<br/>
		/// </summary>
		public IFileEntry GetResourceFile(params string[] pathParts) {
			var pathManager = Application.Ioc.Resolve<LocalPathManager>();
			var fullPath = pathManager.GetResourceFullPath(pathParts);
			return new LocalFileEntry(fullPath, true);
		}

		/// <summary>
		/// Get storage file<br/>
		/// 获取储存文件<br/>
		/// </summary>
		public IFileEntry GetStorageFile(params string[] pathParts) {
			var pathManager = Application.Ioc.Resolve<LocalPathManager>();
			var fullPath = pathManager.GetStorageFullPath(pathParts);
			return new LocalFileEntry(fullPath, false);
		}

		/// <summary>
		/// Get storage directory<br/>
		/// 获取储存目录<br/>
		/// </summary>
		public IDirectoryEntry GetStorageDirectory(params string[] pathParts) {
			var pathManager = Application.Ioc.Resolve<LocalPathManager>();
			var fullPath = pathManager.GetStorageFullPath(pathParts);
			return new LocalDirectoryEntry(fullPath);
		}
	}
}
