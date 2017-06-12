using System.IO;

namespace ZKWeb.Storage {
	/// <summary>
	/// Local path config<br/>
	/// It's better to use IFileStorage<br/>
	/// Unless you really want to access local file system<br/>
	/// 本地路径设置<br/>
	/// 一般情况最好使用IFileStorage, 除非你确实想访问本地文件系统<br/>
	/// </summary>
	public class LocalPathConfig {
		/// <summary>
		/// Website root directory<br/>
		/// 网站根目录的路径<br/>
		/// </summary>
		public virtual string WebsiteRootDirectory { get; protected set; }
		/// <summary>
		/// App_Data directory<br/>
		/// App_Data目录的路径<br/>
		/// </summary>
		public virtual string AppDataDirectory { get; protected set; }
		/// <summary>
		/// Website configuration path<br/>
		/// 网站配置的路径<br/>
		/// </summary>
		public virtual string WebsiteConfigPath { get; protected set; }
		/// <summary>
		/// Filename that store plugin information<br/>
		/// Under plugin directory<br/>
		/// 储存插件信息的文件名<br/>
		/// 在插件目录下<br/>
		/// </summary>
		public virtual string PluginInfoFilename { get; protected set; }
		/// <summary>
		/// Template directory name, only the name<br/>
		/// Under App_Data or plugin directories<br/>
		/// 模板文件夹的文件名, 仅名称<br/>
		/// 在App_Data目录或插件目录下<br/>
		/// </summary>
		public virtual string TemplateDirectoryName { get; protected set; }
		/// <summary>
		/// Device specialized template directory name format (with string.Format)<br/>
		/// Under App_Data or plugin directories<br/>
		/// 设备专用的模板文件名的格式字符串(使用string.Format)<br/>
		/// 在App_Data目录或插件目录下<br/>
		/// </summary>
		public virtual string DeviceSpecializedTemplateDirectoryNameFormat { get; protected set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		internal protected virtual void Initialize(string websiteRootDirectory) {
			var config = Application.Ioc.Resolve<LocalPathConfig>();
			config.WebsiteRootDirectory = Path.GetFullPath(websiteRootDirectory);
			config.AppDataDirectory = Path.Combine(config.WebsiteRootDirectory, "App_Data");
			config.WebsiteConfigPath = Path.Combine(config.AppDataDirectory, "config.json");
			config.PluginInfoFilename = "plugin.json";
			config.TemplateDirectoryName = "templates";
			config.DeviceSpecializedTemplateDirectoryNameFormat = "templates.{0}";
		}
	}
}
