using System.IO;

namespace ZKWeb.Server {
	/// <summary>
	/// Path configuration
	/// </summary>
	public class PathConfig {
		/// <summary>
		/// Website root directory
		/// </summary>
		public virtual string WebsiteRootDirectory { get; protected set; }
		/// <summary>
		/// App_Data directory
		/// </summary>
		public virtual string AppDataDirectory { get; protected set; }
		/// <summary>
		/// Logs directory
		/// </summary>
		public virtual string LogsDirectory { get; protected set; }
		/// <summary>
		/// Website configuration path
		/// </summary>
		public virtual string WebsiteConfigPath { get; protected set; }
		/// <summary>
		/// File that store plugin information
		/// Under plugin directory
		/// </summary>
		public virtual string PluginInfoFilename { get; protected set; }
		/// <summary>
		/// Template directory name, only the name
		/// Under App_Data or plugin directories
		/// </summary>
		public virtual string TemplateDirectoryName { get; protected set; }
		/// <summary>
		/// Device specialized template directory name format (with string.Format)
		/// Under App_Data or plugin directories
		/// </summary>
		public virtual string DeviceSpecializedTemplateDirectoryNameFormat { get; protected set; }

		/// <summary>
		/// Initialize
		/// </summary>
		public static void Initialize(string websiteRootDirectory) {
			var config = Application.Ioc.Resolve<PathConfig>();
			config.WebsiteRootDirectory = Path.GetFullPath(websiteRootDirectory);
			config.AppDataDirectory = Path.Combine(config.WebsiteRootDirectory, "App_Data");
			config.LogsDirectory = Path.Combine(config.AppDataDirectory, "logs");
			config.WebsiteConfigPath = Path.Combine(config.AppDataDirectory, "config.json");
			config.PluginInfoFilename = "plugin.json";
			config.TemplateDirectoryName = "templates";
			config.DeviceSpecializedTemplateDirectoryNameFormat = "templates.{0}";
		}
	}
}
