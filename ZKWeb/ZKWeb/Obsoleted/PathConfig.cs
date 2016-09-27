using System;
using System.IO;
using ZKWeb.Storage;

namespace ZKWeb.Server {
	/// <summary>
	/// This class should no longer be using
	/// Please use IFileStorage or LocalPathConfig
	/// Obsleted in 1.0.2
	/// </summary>
	[Obsolete("This class should no longer be using, please use IFileStorage or LocalPathConfig")]
	public class PathConfig {
#pragma warning disable CS1591
		protected LocalPathConfig Config { get { return Application.Ioc.Resolve<LocalPathConfig>(); } }
		public virtual string WebsiteRootDirectory { get { return Config.WebsiteRootDirectory; } }
		public virtual string AppDataDirectory { get { return Config.AppDataDirectory; } }
		public virtual string LogsDirectory { get { return Path.Combine(AppDataDirectory, "logs"); } }
		public virtual string WebsiteConfigPath { get { return Config.WebsiteConfigPath; } }
		public virtual string PluginInfoFilename { get { return Config.PluginInfoFilename; } }
		public virtual string TemplateDirectoryName { get { return Config.TemplateDirectoryName; } }
		public virtual string DeviceSpecializedTemplateDirectoryNameFormat {
			get { return Config.DeviceSpecializedTemplateDirectoryNameFormat; }
		}
#pragma warning restore CS1591
	}
}
