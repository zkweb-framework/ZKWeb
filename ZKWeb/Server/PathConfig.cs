using System.IO;
using System.Reflection;
using System.Web;
using ZKWebStandard.Web;

namespace ZKWeb.Server {
	/// <summary>
	/// 路径配置
	/// </summary>
	public class PathConfig {
		/// <summary>
		/// 网站根目录
		/// </summary>
		public virtual string WebsiteRootDirectory { get; protected set; }
		/// <summary>
		/// App_Data目录
		/// </summary>
		public virtual string AppDataDirectory { get; protected set; }
		/// <summary>
		/// 日志文件目录
		/// </summary>
		public virtual string LogsDirectory { get; protected set; }
		/// <summary>
		/// 网站配置文件路径
		/// </summary>
		public virtual string WebsiteConfigPath { get; protected set; }
		/// <summary>
		/// 储存插件信息的文件名称
		/// 这个文件会在插件目录下
		/// </summary>
		public virtual string PluginInfoFilename { get; protected set; }
		/// <summary>
		/// 模板文件夹的名称
		/// 这个文件夹可以在App_Data下，也可以在各个插件目录下
		/// </summary>
		public virtual string TemplateDirectoryName { get; protected set; }
		/// <summary>
		/// 设备专用的模板文件夹的名称
		/// 这个文件夹可以在App_Data下，也可以在各个插件目录下
		/// </summary>
		public virtual string DeviceSpecializedTemplateDirectoryNameFormat { get; protected set; }
		/// <summary>
		/// 保存数据表生成脚本的文件路径
		/// 里面的脚本仅用于检测是否需要更新，不会实际执行
		/// </summary>
		public virtual string DatabaseScriptPath { get; protected set; }

		/// <summary>
		/// 初始化
		/// </summary>
		public PathConfig() {
			var websiteRootDirectory = HttpContext.Current?.Server.MapPath("~/");
			if (websiteRootDirectory == null) {
				var consolePath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(IHttpContext)).Location);
				websiteRootDirectory = Path.GetFullPath(Path.Combine(consolePath, "../../../ZKWeb/"));
			}
			WebsiteRootDirectory = websiteRootDirectory;
			AppDataDirectory = Path.Combine(WebsiteRootDirectory, "App_Data");
			LogsDirectory = Path.Combine(AppDataDirectory, "logs");
			WebsiteConfigPath = Path.Combine(AppDataDirectory, "config.json");
			PluginInfoFilename = "plugin.json";
			TemplateDirectoryName = "templates";
			DeviceSpecializedTemplateDirectoryNameFormat = "templates.{0}";
			DatabaseScriptPath = Path.Combine(AppDataDirectory, "DatabaseScript.txt");
		}
	}
}
