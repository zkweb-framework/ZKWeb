using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Server {
	/// <summary>
	/// 路径的默认配置
	/// </summary>
	public static class PathConfig {
		/// <summary>
		/// App_Data目录
		/// </summary>
		public static string AppDataDirectory {
			get { return Path.Combine(PathUtils.WebRoot.Value, "App_Data"); }
		}

		/// <summary>
		/// 数据文件根目录
		/// </summary>
		public static string StorageRootDirectory {
			get { return Path.Combine(PathUtils.WebRoot.Value, "App_Data", "Storage"); }
		}

		/// <summary>
		/// 日志文件目录
		/// </summary>
		public static string LogsDirectory {
			get { return Path.Combine(PathUtils.WebRoot.Value, "App_Data", "Logs"); }
		}

		/// <summary>
		/// 网站配置文件路径
		/// </summary>
		public static string WebsiteConfigPath {
			get { return Path.Combine(PathUtils.WebRoot.Value, "App_Data", "config.json"); }
		}

		/// <summary>
		/// 储存插件信息的文件名称
		/// 这个文件会在插件目录下
		/// </summary>
		public static string PluginInfoFilename { get; } = "plugin.json";

		/// <summary>
		/// 模板文件夹的名称
		/// 这个文件夹可以在App_Data下，也可以在各个插件目录下
		/// </summary>
		public static string TemplateDirectoryName { get; } = "templates";

		/// <summary>
		/// 保存数据表生成脚本的文件路径
		/// 里面的脚本仅用于检测是否需要更新，不会实际执行
		/// </summary>
		public static string DatabaseScriptPath {
			get { return Path.Combine(AppDataDirectory, "DatabaseScript.txt"); }
		}
	}
}
