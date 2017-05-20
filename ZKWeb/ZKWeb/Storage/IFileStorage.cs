namespace ZKWeb.Storage {
	/// <summary>
	/// Interface of file storage<br/>
	/// 文件系统的接口<br/>
	/// </summary>
	/// <example>
	/// Example of reading template file<br/>
	/// 读取模板文件的例子<br/>
	/// <code language="cs">
	/// var fileStorage = Application.Ioc.Resolve&lt;IFileStorage&gt;();
	/// var fileEntry = fileStorage.GetTemplateFile("abc.html");
	/// var contents = fileEntry.ReadAllText();
	/// </code>
	/// 
	/// Example of reading source file<br/>
	/// 获取资源文件的例子<br/>
	/// <code language="cs">
	/// var fileStorage = Application.Ioc.Resolve&lt;IFileStorage&gt;();
	/// var fileEntry = fileStorage.GetResourceFile("abc.jpg");
	/// using (var stream = fileEntry.OpenRead()) { }
	/// </code>
	/// 
	/// Example of saving storage file<br/>
	/// 写入储存文件的例子<br/>
	/// <code language="cs">
	/// var fileStorage = Application.Ioc.Resolve&lt;IFileStorage&gt;();
	/// var fileEntry = fileStorage.GetStorageFile("static", "abc.txt");
	/// fileEntry.WriteAllText("test storage file");
	/// </code>
	/// 
	/// Example of listing files under storage directory (not recursive)<br/>
	/// 列出储存文件夹下的文件的例子(非递归)<br/>
	/// <code language = "cs">
	/// var fileStorage = Application.Ioc.Resolve&lt;IFileStorage&gt;();
	/// var directoryEntry = fileStorage.GetStorageDirectory("static");
	/// var childFiles = directoryEntry.EnumerateFiles();
	/// foreach (var file in childFiles) {
	/// 	console.WriteLine($"{file.Filename}: {file.ReadAllText()}")
	/// }
	/// </code>
	/// </example>
	public interface IFileStorage {
		/// <summary>
		/// Get template file, it should be readonly<br/>
		/// 获取模板文件, 返回的文件应该是只读的<br/>
		/// </summary>
		/// <param name="path">Template file path</param>
		/// <returns></returns>
		IFileEntry GetTemplateFile(string path);

		/// <summary>
		/// Get resource file, it should be readonly<br/>
		/// 获取资源文件, 返回的文件应该是只读的<br/>
		/// </summary>
		/// <param name="pathParts">Resource file path parts</param>
		/// <returns></returns>
		IFileEntry GetResourceFile(params string[] pathParts);

		/// <summary>
		/// Get storage file<br/>
		/// 获取储存文件<br/>
		/// </summary>
		/// <param name="pathParts">Storage file path parts</param>
		/// <returns></returns>
		IFileEntry GetStorageFile(params string[] pathParts);

		/// <summary>
		/// Get storage directory<br/>
		/// 获取储存目录<br/>
		/// </summary>
		/// <param name="pathParts">Storage directory path parts</param>
		IDirectoryEntry GetStorageDirectory(params string[] pathParts);
	}
}
