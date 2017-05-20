using System;
using System.IO;
using ZKWeb.Storage;

namespace ZKWeb.Server {
	/// <summary>
	/// This class should no longer be used<br/>
	/// Please use IFileStorage or LocalPathConfig<br/>
	/// 这个类不应该再被使用, 请使用IFileStorage或LocalPathConfig<br/>
	/// </summary>
	[Obsolete("This class should no longer be used, please use IFileStorage or LocalPathConfig")]
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
